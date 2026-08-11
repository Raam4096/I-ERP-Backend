#!/usr/bin/env python3
from __future__ import annotations
from pathlib import Path
from textwrap import dedent

ROOT = Path(__file__).resolve().parents[1]

def write(rel: str, content: str) -> None:
    path = ROOT / rel
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(dedent(content).lstrip("\n").replace("\r\n", "\n"), encoding="utf-8")
    print(rel)

# Fix Platform DbContexts
write("src/Modules/Platform/iERP.Modules.Platform/Tenancy/Infrastructure/PlatformDbContext.cs", """
using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.Attachments.Domain;
using iERP.Modules.Platform.Audit.Domain;
using iERP.Modules.Platform.DynamicModules.Domain;
using iERP.Modules.Platform.Notifications.Domain;
using iERP.Modules.Platform.Tenancy.Domain;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.Platform.Tenancy.Infrastructure;

public sealed class PlatformDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public PlatformDbContext(DbContextOptions<PlatformDbContext> options, ITenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<NotificationLog> NotificationLogs => Set<NotificationLog>();
    public DbSet<DynamicModuleDefinition> DynamicModuleDefinitions => Set<DynamicModuleDefinition>();
    public DbSet<DynamicEntityDefinition> DynamicEntityDefinitions => Set<DynamicEntityDefinition>();
    public DbSet<DynamicFieldDefinition> DynamicFieldDefinitions => Set<DynamicFieldDefinition>();
    public DbSet<DynamicRecord> DynamicRecords => Set<DynamicRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var assembly = typeof(PlatformDbContext).Assembly;
        modelBuilder.HasDefaultSchema("platform");
        modelBuilder.ApplyConfigurationsFromNamespace(assembly, "iERP.Modules.Platform.Tenancy.Infrastructure.Configurations");
        modelBuilder.ApplyConfigurationsFromNamespace(assembly, "iERP.Modules.Platform.Audit.Infrastructure.Configurations");
        modelBuilder.ApplyConfigurationsFromNamespace(assembly, "iERP.Modules.Platform.Attachments.Infrastructure.Configurations");
        modelBuilder.ApplyConfigurationsFromNamespace(assembly, "iERP.Modules.Platform.Notifications.Infrastructure.Configurations");
        modelBuilder.ApplyConfigurationsFromNamespace(assembly, "iERP.Modules.Platform.DynamicModules.Infrastructure.Configurations");
        modelBuilder.ApplySnakeCaseNamingConvention();
        modelBuilder.ConfigureMoneyPrecision();
        modelBuilder.ApplyTenantAndSoftDeleteFilters(_tenantContext);
        base.OnModelCreating(modelBuilder);
    }
}
""")

write("src/Modules/Platform/iERP.Modules.Platform/Identity/Infrastructure/IdentityDbContext.cs", """
using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.Identity.Domain;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.Platform.Identity.Infrastructure;

public sealed class IdentityDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public IdentityDbContext(DbContextOptions<IdentityDbContext> options, ITenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<AppRole> Roles => Set<AppRole>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserSubsidiary> UserSubsidiaries => Set<UserSubsidiary>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<FieldPermissionGrant> FieldPermissionGrants => Set<FieldPermissionGrant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("identity");
        modelBuilder.ApplyConfigurationsFromNamespace(
            typeof(IdentityDbContext).Assembly,
            "iERP.Modules.Platform.Identity.Infrastructure.Configurations");
        modelBuilder.ApplySnakeCaseNamingConvention();
        modelBuilder.ConfigureMoneyPrecision();
        modelBuilder.ApplyTenantAndSoftDeleteFilters(_tenantContext);
        base.OnModelCreating(modelBuilder);
    }
}
""")

write("src/Modules/Platform/iERP.Modules.Platform/Organization/Infrastructure/OrganizationDbContext.cs", """
using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.Organization.Domain;
using iERP.Modules.Platform.Settings.Domain;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.Platform.Organization.Infrastructure;

public sealed class OrganizationDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public OrganizationDbContext(DbContextOptions<OrganizationDbContext> options, ITenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<Subsidiary> Subsidiaries => Set<Subsidiary>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<CostCenter> CostCenters => Set<CostCenter>();
    public DbSet<ReportingDimension> ReportingDimensions => Set<ReportingDimension>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
    public DbSet<DocumentSequence> DocumentSequences => Set<DocumentSequence>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var assembly = typeof(OrganizationDbContext).Assembly;
        modelBuilder.HasDefaultSchema("organization");
        modelBuilder.ApplyConfigurationsFromNamespace(assembly, "iERP.Modules.Platform.Organization.Infrastructure.Configurations");
        modelBuilder.ApplyConfigurationsFromNamespace(assembly, "iERP.Modules.Platform.Settings.Infrastructure.Configurations");
        modelBuilder.ApplySnakeCaseNamingConvention();
        modelBuilder.ConfigureMoneyPrecision();
        modelBuilder.ApplyTenantAndSoftDeleteFilters(_tenantContext);
        base.OnModelCreating(modelBuilder);
    }
}
""")

write("src/Modules/Platform/iERP.Modules.Platform/Metadata/Infrastructure/MetadataDbContext.cs", """
using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.Metadata.Domain;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.Platform.Metadata.Infrastructure;

public sealed class MetadataDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public MetadataDbContext(DbContextOptions<MetadataDbContext> options, ITenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<ModuleDefinition> ModuleDefinitions => Set<ModuleDefinition>();
    public DbSet<ScreenDefinition> ScreenDefinitions => Set<ScreenDefinition>();
    public DbSet<SectionDefinition> SectionDefinitions => Set<SectionDefinition>();
    public DbSet<FieldDefinition> FieldDefinitions => Set<FieldDefinition>();
    public DbSet<CustomFieldDefinition> CustomFieldDefinitions => Set<CustomFieldDefinition>();
    public DbSet<CustomFieldValue> CustomFieldValues => Set<CustomFieldValue>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("metadata");
        modelBuilder.ApplyConfigurationsFromNamespace(
            typeof(MetadataDbContext).Assembly,
            "iERP.Modules.Platform.Metadata.Infrastructure.Configurations");
        modelBuilder.ApplySnakeCaseNamingConvention();
        modelBuilder.ConfigureMoneyPrecision();
        modelBuilder.ApplyTenantAndSoftDeleteFilters(_tenantContext);
        base.OnModelCreating(modelBuilder);
    }
}
""")

# Fix Engines DbContexts
for area, ctx, schema, sets in [
    ("Workflow", "WorkflowDbContext", "workflow", """
    public DbSet<WorkflowDefinition> WorkflowDefinitions => Set<WorkflowDefinition>();
    public DbSet<WorkflowStep> WorkflowSteps => Set<WorkflowStep>();
    public DbSet<WorkflowInstance> WorkflowInstances => Set<WorkflowInstance>();
    public DbSet<WorkflowHistory> WorkflowHistories => Set<WorkflowHistory>();
"""),
    ("Rules", "RulesDbContext", "rules", """
    public DbSet<RuleDefinition> RuleDefinitions => Set<RuleDefinition>();
"""),
    ("Bridge", "BridgeDbContext", "bridge", """
    public DbSet<BridgeDefinition> BridgeDefinitions => Set<BridgeDefinition>();
    public DbSet<BridgeMapping> BridgeMappings => Set<BridgeMapping>();
    public DbSet<BridgeLog> BridgeLogs => Set<BridgeLog>();
"""),
    ("Printing", "PrintingDbContext", "printing", """
    public DbSet<PrintTemplate> PrintTemplates => Set<PrintTemplate>();
    public DbSet<PrintTemplateVersion> PrintTemplateVersions => Set<PrintTemplateVersion>();
"""),
]:
    write(f"src/Modules/Engines/iERP.Modules.Engines/{area}/Infrastructure/{ctx}.cs", f"""
using iERP.Infrastructure.Persistence;
using iERP.Modules.Engines.{area}.Domain;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.Engines.{area}.Infrastructure;

public sealed class {ctx} : DbContext
{{
    private readonly ITenantContext _tenantContext;

    public {ctx}(DbContextOptions<{ctx}> options, ITenantContext tenantContext) : base(options)
    {{
        _tenantContext = tenantContext;
    }}
{sets}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {{
        modelBuilder.HasDefaultSchema("{schema}");
        modelBuilder.ApplyConfigurationsFromNamespace(
            typeof({ctx}).Assembly,
            "iERP.Modules.Engines.{area}.Infrastructure.Configurations");
        modelBuilder.ApplySnakeCaseNamingConvention();
        modelBuilder.ConfigureMoneyPrecision();
        modelBuilder.ApplyTenantAndSoftDeleteFilters(_tenantContext);
        base.OnModelCreating(modelBuilder);
    }}
}}
""")

# Fix Infrastructure DI missing usings
write("src/BuildingBlocks/iERP.Infrastructure/DependencyInjection.cs", """
using System.Text;
using System.Threading.RateLimiting;
using iERP.Application.Abstractions.AI;
using iERP.Application.Abstractions.Caching;
using iERP.Application.Abstractions.Jobs;
using iERP.Application.Abstractions.Messaging;
using iERP.Application.Abstractions.Notifications;
using iERP.Application.Abstractions.Options;
using iERP.Application.Abstractions.Reporting;
using iERP.Application.Abstractions.Storage;
using iERP.Infrastructure.AI;
using iERP.Infrastructure.Caching;
using iERP.Infrastructure.Exceptions;
using iERP.Infrastructure.Jobs;
using iERP.Infrastructure.Messaging;
using iERP.Infrastructure.Notifications;
using iERP.Infrastructure.Persistence.Interceptors;
using iERP.Infrastructure.Reporting;
using iERP.Infrastructure.Storage;
using iERP.Infrastructure.Tenancy;
using iERP.SharedKernel.Tenancy;
using iERP.SharedKernel.Time;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
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
        services.AddSingleton<IClock, SystemClock>();
        services.AddScoped<ITenantContext, TenantContext>();
        services.AddScoped<ITenantResolver, ClaimTenantResolver>();
        services.AddScoped<TenantSaveChangesInterceptor>();
        services.AddScoped<AuditSaveChangesInterceptor>();

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        AddAuth(services, configuration);
        AddCache(services, configuration);
        AddAzureAbstractions(services, configuration);
        AddObservability(services, configuration);
        AddHealthChecks(services, configuration);
        AddRateLimiting(services);

        services.AddScoped<IBackgroundJobService, HangfireBackgroundJobService>();
        services.AddScoped<IReportingDbConnectionFactory, ReportingDbConnectionFactory>();

        return services;
    }

    private static void AddAuth(IServiceCollection services, IConfiguration configuration)
    {
        var jwt = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
        var signingKey = string.IsNullOrWhiteSpace(jwt.SigningKey)
            ? "LOCAL_DEV_ONLY_CHANGE_ME_TO_A_LONG_RANDOM_SECRET_KEY"
            : jwt.SigningKey;

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            });

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
        services.AddSingleton<IEventBus, NullEventBus>();
        services.AddSingleton<IIntegrationEventPublisher>(sp => (NullEventBus)sp.GetRequiredService<IEventBus>());

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

    private static void AddRateLimiting(IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 200,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    }));
        });
    }
}
""")

# FluentValidation example
write("src/Modules/CRM/iERP.Modules.CRM/Application/Validation/CreateLeadPlaceholderValidator.cs", """
using FluentValidation;

namespace iERP.Modules.CRM.Application.Validation;

/// <summary>
/// Placeholder demonstrating FluentValidation structure for future APIs.
/// </summary>
public sealed class CreateLeadPlaceholderRequest
{
    public string? LeadNumber { get; set; }
    public Guid SubsidiaryId { get; set; }
}

public sealed class CreateLeadPlaceholderValidator : AbstractValidator<CreateLeadPlaceholderRequest>
{
    public CreateLeadPlaceholderValidator()
    {
        RuleFor(x => x.LeadNumber).NotEmpty().MaximumLength(64);
        RuleFor(x => x.SubsidiaryId).NotEmpty();
    }
}
""")

# API Program
write("src/iERP.Api/Program.cs", """
using Hangfire;
using Hangfire.PostgreSql;
using HealthChecks.UI.Client;
using iERP.Application.Abstractions.Options;
using iERP.Infrastructure;
using iERP.Infrastructure.Middleware;
using iERP.Infrastructure.Tenancy;
using iERP.Modules.AI;
using iERP.Modules.AI.Api;
using iERP.Modules.Assets;
using iERP.Modules.Assets.Api;
using iERP.Modules.Banking;
using iERP.Modules.Banking.Api;
using iERP.Modules.Catalog;
using iERP.Modules.Catalog.Api;
using iERP.Modules.CRM;
using iERP.Modules.CRM.Api;
using iERP.Modules.Engines;
using iERP.Modules.Engines.Api;
using iERP.Modules.Finance;
using iERP.Modules.Finance.Api;
using iERP.Modules.HR;
using iERP.Modules.HR.Api;
using iERP.Modules.Inventory;
using iERP.Modules.Inventory.Api;
using iERP.Modules.Manufacturing;
using iERP.Modules.Manufacturing.Api;
using iERP.Modules.Marine;
using iERP.Modules.Marine.Api;
using iERP.Modules.Platform;
using iERP.Modules.Platform.Api;
using iERP.Modules.Procurement;
using iERP.Modules.Procurement.Api;
using iERP.Modules.Projects;
using iERP.Modules.Projects.Api;
using iERP.Modules.Reporting;
using iERP.Modules.Reporting.Api;
using iERP.Modules.Sales;
using iERP.Modules.Sales.Api;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIerpInfrastructure(builder.Configuration);
builder.Services.AddPlatformModule(builder.Configuration);
builder.Services.AddEnginesModule(builder.Configuration);
builder.Services.AddCrmModule(builder.Configuration);
builder.Services.AddCatalogModule(builder.Configuration);
builder.Services.AddSalesModule(builder.Configuration);
builder.Services.AddProcurementModule(builder.Configuration);
builder.Services.AddInventoryModule(builder.Configuration);
builder.Services.AddFinanceModule(builder.Configuration);
builder.Services.AddBankingModule(builder.Configuration);
builder.Services.AddProjectsModule(builder.Configuration);
builder.Services.AddHrModule(builder.Configuration);
builder.Services.AddManufacturingModule(builder.Configuration);
builder.Services.AddAssetsModule(builder.Configuration);
builder.Services.AddMarineModule(builder.Configuration);
builder.Services.AddReportingModule(builder.Configuration);
builder.Services.AddAiModule(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "i-ERP API", Version = "v1" });
});

var hangfireOptions = builder.Configuration.GetSection(HangfireOptions.SectionName).Get<HangfireOptions>() ?? new HangfireOptions();
var connectionString = builder.Configuration.GetConnectionString("PrimaryDatabase");
if (hangfireOptions.Enabled && !string.IsNullOrWhiteSpace(connectionString))
{
    builder.Services.AddHangfire(config => config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(connectionString)));
    builder.Services.AddHangfireServer(options => options.WorkerCount = hangfireOptions.WorkerCount);
}

var app = builder.Build();

app.UseExceptionHandler();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseMiddleware<TenantResolutionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

if (hangfireOptions.Enabled && !string.IsNullOrWhiteSpace(connectionString))
{
    app.UseHangfireDashboard("/hangfire");
}

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("live")
});
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("ready")
});
app.MapHealthChecks("/health");

app.MapPlatformEndpoints();
app.MapEnginesEndpoints();
app.MapCrmEndpoints();
app.MapCustomerEndpoints();
app.MapCatalogEndpoints();
app.MapSalesEndpoints();
app.MapSalesExtraEndpoints();
app.MapProcurementEndpoints();
app.MapInventoryEndpoints();
app.MapFinanceEndpoints();
app.MapBankingEndpoints();
app.MapProjectsEndpoints();
app.MapHrEndpoints();
app.MapManufacturingEndpoints();
app.MapAssetsEndpoints();
app.MapMarineEndpoints();
app.MapReportingEndpoints();
app.MapAiEndpoints();

app.Run();

public partial class Program;
""")

write("src/iERP.Api/appsettings.json", """
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "PrimaryDatabase": "Host=localhost;Port=5432;Database=ierp;Username=ierp;Password=ierp",
    "ReportingDatabase": "Host=localhost;Port=5432;Database=ierp;Username=ierp;Password=ierp"
  },
  "Jwt": {
    "Issuer": "i-ERP",
    "Audience": "i-ERP",
    "SigningKey": "LOCAL_DEV_ONLY_CHANGE_ME_TO_A_LONG_RANDOM_SECRET_KEY",
    "AccessTokenMinutes": 30,
    "RefreshTokenDays": 14
  },
  "Redis": {
    "Enabled": false,
    "ConnectionString": "localhost:6379",
    "InstanceName": "ierp:"
  },
  "AzureServiceBus": {
    "Enabled": false,
    "ConnectionString": "",
    "TopicName": "ierp-events"
  },
  "AzureOpenAI": {
    "Enabled": false,
    "Endpoint": "",
    "ApiKey": "",
    "DeploymentName": ""
  },
  "AzureBlobStorage": {
    "Enabled": false,
    "ConnectionString": "",
    "ContainerName": "ierp-attachments"
  },
  "Hangfire": {
    "Enabled": false,
    "SchemaName": "hangfire",
    "WorkerCount": 2
  },
  "OpenTelemetry": {
    "ServiceName": "iERP.Api"
  },
  "ApplicationInsights": {
    "ConnectionString": ""
  }
}
""")

write("src/iERP.Api/appsettings.Development.json", """
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  },
  "Hangfire": {
    "Enabled": false
  },
  "Redis": {
    "Enabled": false
  }
}
""")

write("src/iERP.Api/Properties/launchSettings.json", """
{
  "profiles": {
    "iERP.Api": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "http://localhost:5080",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
""")

# Worker
write("src/iERP.Worker/Program.cs", """
using Hangfire;
using Hangfire.PostgreSql;
using iERP.Application.Abstractions.Options;
using iERP.Infrastructure;
using iERP.Modules.Platform;
using iERP.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddIerpInfrastructure(builder.Configuration);
builder.Services.AddPlatformModule(builder.Configuration);

var hangfireOptions = builder.Configuration.GetSection(HangfireOptions.SectionName).Get<HangfireOptions>() ?? new HangfireOptions();
var connectionString = builder.Configuration.GetConnectionString("PrimaryDatabase");

if (hangfireOptions.Enabled && !string.IsNullOrWhiteSpace(connectionString))
{
    builder.Services.AddHangfire(config => config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(connectionString)));
    builder.Services.AddHangfireServer(options => options.WorkerCount = hangfireOptions.WorkerCount);
}

builder.Services.AddHostedService<OutboxProcessorWorker>();

var host = builder.Build();
host.Run();
""")

write("src/iERP.Worker/OutboxProcessorWorker.cs", """
using iERP.Modules.Platform.Tenancy.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace iERP.Worker;

/// <summary>
/// Skeleton worker that will later publish outbox messages to Azure Service Bus.
/// </summary>
public sealed class OutboxProcessorWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxProcessorWorker> _logger;

    public OutboxProcessorWorker(IServiceScopeFactory scopeFactory, ILogger<OutboxProcessorWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox processor worker started (placeholder).");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<PlatformDbContext>();
                var pending = await db.OutboxMessages
                    .IgnoreQueryFilters()
                    .Where(x => x.ProcessedAt == null)
                    .OrderBy(x => x.OccurredAt)
                    .Take(50)
                    .ToListAsync(stoppingToken);

                if (pending.Count > 0)
                {
                    _logger.LogDebug("Found {Count} pending outbox messages (not published yet).", pending.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Outbox poll skipped (database may be unavailable).");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
""")

write("src/iERP.Worker/appsettings.json", """
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "ConnectionStrings": {
    "PrimaryDatabase": "Host=localhost;Port=5432;Database=ierp;Username=ierp;Password=ierp",
    "ReportingDatabase": "Host=localhost;Port=5432;Database=ierp;Username=ierp;Password=ierp"
  },
  "Hangfire": {
    "Enabled": false,
    "WorkerCount": 2
  },
  "Redis": {
    "Enabled": false
  },
  "Jwt": {
    "Issuer": "i-ERP",
    "Audience": "i-ERP",
    "SigningKey": "LOCAL_DEV_ONLY_CHANGE_ME_TO_A_LONG_RANDOM_SECRET_KEY"
  },
  "AzureServiceBus": { "Enabled": false },
  "AzureOpenAI": { "Enabled": false },
  "AzureBlobStorage": { "Enabled": false }
}
""")

write("src/iERP.Worker/appsettings.Development.json", """
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  }
}
""")

# Migrations helpers
write("src/iERP.Migrations/DesignTime/DesignTimeDbContextFactoryBase.cs", """
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace iERP.Migrations.DesignTime;

public abstract class DesignTimeDbContextFactoryBase<TContext> : IDesignTimeDbContextFactory<TContext>
    where TContext : DbContext
{
    protected abstract TContext Create(DbContextOptions<TContext> options, ITenantContext tenantContext);

    public TContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "iERP.Api"))
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("PrimaryDatabase")
            ?? "Host=localhost;Port=5432;Database=ierp;Username=ierp;Password=ierp";

        var optionsBuilder = new DbContextOptionsBuilder<TContext>();
        optionsBuilder.UseNpgsql(connectionString, b => b.MigrationsAssembly("iERP.Migrations"));

        var tenantContext = new TenantContext();
        return Create(optionsBuilder.Options, tenantContext);
    }
}
""")

# Create factories for major contexts
contexts = [
    ("iERP.Modules.Platform.Tenancy.Infrastructure", "PlatformDbContext"),
    ("iERP.Modules.Platform.Identity.Infrastructure", "IdentityDbContext"),
    ("iERP.Modules.Platform.Organization.Infrastructure", "OrganizationDbContext"),
    ("iERP.Modules.Platform.Metadata.Infrastructure", "MetadataDbContext"),
    ("iERP.Modules.Engines.Workflow.Infrastructure", "WorkflowDbContext"),
    ("iERP.Modules.Engines.Rules.Infrastructure", "RulesDbContext"),
    ("iERP.Modules.Engines.Bridge.Infrastructure", "BridgeDbContext"),
    ("iERP.Modules.Engines.Printing.Infrastructure", "PrintingDbContext"),
    ("iERP.Modules.CRM.Infrastructure", "CrmDbContext"),
    ("iERP.Modules.Catalog.Infrastructure", "CatalogDbContext"),
    ("iERP.Modules.Sales.Infrastructure", "SalesDbContext"),
    ("iERP.Modules.Procurement.Infrastructure", "ProcurementDbContext"),
    ("iERP.Modules.Inventory.Infrastructure", "InventoryDbContext"),
    ("iERP.Modules.Finance.Infrastructure", "FinanceDbContext"),
    ("iERP.Modules.Banking.Infrastructure", "BankingDbContext"),
    ("iERP.Modules.Projects.Infrastructure", "ProjectsDbContext"),
    ("iERP.Modules.HR.Infrastructure", "HrDbContext"),
    ("iERP.Modules.Manufacturing.Infrastructure", "ManufacturingDbContext"),
    ("iERP.Modules.Assets.Infrastructure", "AssetsDbContext"),
    ("iERP.Modules.Marine.Infrastructure", "MarineDbContext"),
    ("iERP.Modules.Reporting.Infrastructure", "ReportingDbContext"),
    ("iERP.Modules.AI.Infrastructure", "AiDbContext"),
]

for ns, name in contexts:
    write(f"src/iERP.Migrations/DesignTime/{name}Factory.cs", f"""
using {ns};
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Migrations.DesignTime;

public sealed class {name}Factory : DesignTimeDbContextFactoryBase<{name}>
{{
    protected override {name} Create(DbContextOptions<{name}> options, ITenantContext tenantContext)
        => new(options, tenantContext);
}}
""")

write("src/iERP.Migrations/README.md", """
# iERP Migrations

Central migrations assembly for all module DbContexts.

Each DbContext owns its PostgreSQL schema and migration history table can be shared or separated by context.

## Add migration (example)

```bash
dotnet ef migrations add InitialPlatform --project src/iERP.Migrations --startup-project src/iERP.Api --context PlatformDbContext --output-dir Migrations/Platform
dotnet ef migrations add InitialIdentity --project src/iERP.Migrations --startup-project src/iERP.Api --context IdentityDbContext --output-dir Migrations/Identity
dotnet ef migrations add InitialOrganization --project src/iERP.Migrations --startup-project src/iERP.Api --context OrganizationDbContext --output-dir Migrations/Organization
dotnet ef migrations add InitialMetadata --project src/iERP.Migrations --startup-project src/iERP.Api --context MetadataDbContext --output-dir Migrations/Metadata
dotnet ef migrations add InitialCrm --project src/iERP.Migrations --startup-project src/iERP.Api --context CrmDbContext --output-dir Migrations/Crm
```

Repeat for each context. See `docs/database-migrations.md`.
""")

# Need Microsoft.Extensions.Configuration.Json for Migrations
write("src/iERP.Migrations/iERP.Migrations.csproj", """
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <RootNamespace>iERP.Migrations</RootNamespace>
    <AssemblyName>iERP.Migrations</AssemblyName>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.Extensions.Configuration.Json" />
    <PackageReference Include="Microsoft.Extensions.Configuration.EnvironmentVariables" />
    <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\\BuildingBlocks\\iERP.Infrastructure\\iERP.Infrastructure.csproj" />
    <ProjectReference Include="..\\Modules\\Platform\\iERP.Modules.Platform\\iERP.Modules.Platform.csproj" />
    <ProjectReference Include="..\\Modules\\Engines\\iERP.Modules.Engines\\iERP.Modules.Engines.csproj" />
    <ProjectReference Include="..\\Modules\\CRM\\iERP.Modules.CRM\\iERP.Modules.CRM.csproj" />
    <ProjectReference Include="..\\Modules\\Catalog\\iERP.Modules.Catalog\\iERP.Modules.Catalog.csproj" />
    <ProjectReference Include="..\\Modules\\Sales\\iERP.Modules.Sales\\iERP.Modules.Sales.csproj" />
    <ProjectReference Include="..\\Modules\\Procurement\\iERP.Modules.Procurement\\iERP.Modules.Procurement.csproj" />
    <ProjectReference Include="..\\Modules\\Inventory\\iERP.Modules.Inventory\\iERP.Modules.Inventory.csproj" />
    <ProjectReference Include="..\\Modules\\Finance\\iERP.Modules.Finance\\iERP.Modules.Finance.csproj" />
    <ProjectReference Include="..\\Modules\\Banking\\iERP.Modules.Banking\\iERP.Modules.Banking.csproj" />
    <ProjectReference Include="..\\Modules\\Projects\\iERP.Modules.Projects\\iERP.Modules.Projects.csproj" />
    <ProjectReference Include="..\\Modules\\HR\\iERP.Modules.HR\\iERP.Modules.HR.csproj" />
    <ProjectReference Include="..\\Modules\\Manufacturing\\iERP.Modules.Manufacturing\\iERP.Modules.Manufacturing.csproj" />
    <ProjectReference Include="..\\Modules\\Assets\\iERP.Modules.Assets\\iERP.Modules.Assets.csproj" />
    <ProjectReference Include="..\\Modules\\Marine\\iERP.Modules.Marine\\iERP.Modules.Marine.csproj" />
    <ProjectReference Include="..\\Modules\\Reporting\\iERP.Modules.Reporting\\iERP.Modules.Reporting.csproj" />
    <ProjectReference Include="..\\Modules\\AI\\iERP.Modules.AI\\iERP.Modules.AI.csproj" />
  </ItemGroup>
</Project>
""")

# Add package versions
print("hosts partial done")
