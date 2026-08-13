using System.Text;
using iERP.Application.Abstractions.AI;
using iERP.Application.Abstractions.Caching;
using iERP.Application.Abstractions.Messaging;
using iERP.Application.Abstractions.Notifications;
using iERP.Application.Abstractions.Options;
using iERP.Application.Abstractions.Reporting;
using iERP.Application.Abstractions.Storage;
using iERP.Infrastructure.AI;
using iERP.Infrastructure.Caching;
using iERP.Infrastructure.Exceptions;
using iERP.Infrastructure.Messaging;
using iERP.Infrastructure.Notifications;
using iERP.Infrastructure.Persistence.Interceptors;
using iERP.Infrastructure.Reporting;
using iERP.Infrastructure.Storage;
using iERP.Infrastructure.Security;
using iERP.Infrastructure.Tenancy;
using iERP.SharedKernel.Security;
using iERP.SharedKernel.Tenancy;
using iERP.SharedKernel.Time;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace iERP.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIerpInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(options =>
        {
            options.PrimaryDatabase = configuration.GetConnectionString("PrimaryDatabase") ?? string.Empty;
            options.ReportingDatabase = configuration.GetConnectionString("ReportingDatabase") ?? string.Empty;
        });
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<RedisOptions>(configuration.GetSection(RedisOptions.SectionName));
        services.Configure<AzureServiceBusOptions>(configuration.GetSection(AzureServiceBusOptions.SectionName));
        services.Configure<AzureOpenAIOptions>(configuration.GetSection(AzureOpenAIOptions.SectionName));
        services.Configure<AzureBlobStorageOptions>(configuration.GetSection(AzureBlobStorageOptions.SectionName));
        services.Configure<HangfireOptions>(configuration.GetSection(HangfireOptions.SectionName));

        services.AddHttpContextAccessor();
        services.AddSingleton<IClock, iERP.SharedKernel.Time.SystemClock>();
        services.AddScoped<ITenantContext, TenantContext>();
        services.AddScoped<ITenantResolver, ClaimTenantResolver>();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<TenantSaveChangesInterceptor>();
        services.AddScoped<AuditSaveChangesInterceptor>();

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        AddAuth(services, configuration);
        AddCache(services, configuration);
        AddAzureAbstractions(services, configuration);
        AddObservability(services, configuration);
        AddHealthChecks(services, configuration);

        services.AddScoped<IReportingDbConnectionFactory, ReportingDbConnectionFactory>();

        return services;
    }

    private static void AddAuth(IServiceCollection services, IConfiguration configuration)
    {
        var jwt = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
        var signingKey = string.IsNullOrWhiteSpace(jwt.SigningKey)
            ? "LOCAL_DEV_ONLY_CHANGE_ME_TO_A_LONG_RANDOM_SECRET_KEY"
            : jwt.SigningKey;

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Smart";
                options.DefaultChallengeScheme = "Smart";
            })
            .AddPolicyScheme("Smart", "JWT or Development", options =>
            {
                options.ForwardDefaultSelector = context =>
                {
                    var header = context.Request.Headers.Authorization.ToString();
                    if (!string.IsNullOrWhiteSpace(header) &&
                        header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    {
                        return JwtBearerDefaults.AuthenticationScheme;
                    }

                    var env = context.RequestServices.GetService<IWebHostEnvironment>();
                    return env?.IsDevelopment() == true
                        ? DevelopmentAuthenticationHandler.SchemeName
                        : JwtBearerDefaults.AuthenticationScheme;
                };
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            })
            .AddScheme<AuthenticationSchemeOptions, DevelopmentAuthenticationHandler>(
                DevelopmentAuthenticationHandler.SchemeName,
                _ => { });

        services.AddAuthorization();
    }

    private static void AddCache(IServiceCollection services, IConfiguration configuration)
    {
        var redis = configuration.GetSection(RedisOptions.SectionName).Get<RedisOptions>() ?? new RedisOptions();
        if (redis.Enabled)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redis.ConnectionString;
                options.InstanceName = redis.InstanceName;
            });
            services.AddSingleton<ICacheService, RedisCacheService>();
        }
        else
        {
            services.AddSingleton<ICacheService, NullCacheService>();
        }
    }

    private static void AddAzureAbstractions(IServiceCollection services, IConfiguration configuration)
    {
        _ = configuration.GetSection(AzureServiceBusOptions.SectionName).Get<AzureServiceBusOptions>() ?? new AzureServiceBusOptions();
        services.AddSingleton<NullEventBus>();
        services.AddSingleton<IEventBus>(sp => sp.GetRequiredService<NullEventBus>());
        services.AddSingleton<IIntegrationEventPublisher>(sp => sp.GetRequiredService<NullEventBus>());

        var blob = configuration.GetSection(AzureBlobStorageOptions.SectionName).Get<AzureBlobStorageOptions>() ?? new AzureBlobStorageOptions();
        if (blob.Enabled && !string.IsNullOrWhiteSpace(blob.ConnectionString))
        {
            services.AddSingleton<IFileStorage, AzureBlobFileStorage>();
        }
        else
        {
            services.AddSingleton<IFileStorage, NullFileStorage>();
        }

        var openAi = configuration.GetSection(AzureOpenAIOptions.SectionName).Get<AzureOpenAIOptions>() ?? new AzureOpenAIOptions();
        if (openAi.Enabled)
        {
            services.AddSingleton<ILLMProvider, AzureOpenAIProvider>();
        }
        else
        {
            services.AddSingleton<ILLMProvider, NullLLMProvider>();
        }

        services.AddSingleton<IEmailSender, NullEmailSender>();
        services.AddSingleton<IWhatsAppSender, NullWhatsAppSender>();
        services.AddSingleton<INotificationService, NullNotificationService>();
    }

    private static void AddObservability(IServiceCollection services, IConfiguration configuration)
    {
        var serviceName = configuration["OpenTelemetry:ServiceName"] ?? "iERP.Api";
        services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService(serviceName))
            .WithTracing(t => t
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation())
            .WithMetrics(m => m
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation());
    }

    private static void AddHealthChecks(IServiceCollection services, IConfiguration configuration)
    {
        var primary = configuration.GetConnectionString("PrimaryDatabase") ?? string.Empty;
        var redis = configuration.GetSection(RedisOptions.SectionName).Get<RedisOptions>() ?? new RedisOptions();

        var builder = services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"]);

        if (!string.IsNullOrWhiteSpace(primary))
        {
            builder.AddNpgSql(primary, name: "postgres", tags: ["ready"]);
        }

        if (redis.Enabled && !string.IsNullOrWhiteSpace(redis.ConnectionString))
        {
            builder.AddRedis(redis.ConnectionString, name: "redis", tags: ["ready"]);
        }
    }

    }
