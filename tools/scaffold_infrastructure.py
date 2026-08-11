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

BASE = "src/BuildingBlocks/iERP.Infrastructure"

write(f"{BASE}/Persistence/SnakeCaseNameConverter.cs", """
using System.Text.RegularExpressions;

namespace iERP.Infrastructure.Persistence;

public static partial class SnakeCaseNameConverter
{
    public static string ToSnakeCase(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return name;
        }

        var snake = CamelBoundary().Replace(name, "$1_$2");
        return snake.ToLowerInvariant();
    }

    [GeneratedRegex("([a-z0-9])([A-Z])")]
    private static partial Regex CamelBoundary();
}
""")

write(f"{BASE}/Persistence/ModelBuilderExtensions.cs", """
using iERP.SharedKernel.Primitives;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace iERP.Infrastructure.Persistence;

public static class ModelBuilderExtensions
{
    public static void ApplySnakeCaseNamingConvention(this ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entity.GetTableName();
            if (!string.IsNullOrWhiteSpace(tableName))
            {
                entity.SetTableName(SnakeCaseNameConverter.ToSnakeCase(tableName));
            }

            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(SnakeCaseNameConverter.ToSnakeCase(property.Name));
            }

            foreach (var key in entity.GetKeys())
            {
                var keyName = key.GetName();
                if (!string.IsNullOrWhiteSpace(keyName))
                {
                    key.SetName(SnakeCaseNameConverter.ToSnakeCase(keyName));
                }
            }

            foreach (var index in entity.GetIndexes())
            {
                var indexName = index.GetDatabaseName();
                if (!string.IsNullOrWhiteSpace(indexName))
                {
                    index.SetDatabaseName(SnakeCaseNameConverter.ToSnakeCase(indexName));
                }
            }

            foreach (var fk in entity.GetForeignKeys())
            {
                var fkName = fk.GetConstraintName();
                if (!string.IsNullOrWhiteSpace(fkName))
                {
                    fk.SetConstraintName(SnakeCaseNameConverter.ToSnakeCase(fkName));
                }
            }
        }
    }

    public static void ApplyTenantAndSoftDeleteFilters(this ModelBuilder modelBuilder, ITenantContext tenantContext)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            if (!typeof(ITenantEntity).IsAssignableFrom(clrType) || !typeof(ISoftDeletable).IsAssignableFrom(clrType))
            {
                continue;
            }

            var method = typeof(ModelBuilderExtensions)
                .GetMethod(nameof(SetTenantSoftDeleteFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                .MakeGenericMethod(clrType);

            method.Invoke(null, [modelBuilder, tenantContext]);
        }
    }

    private static void SetTenantSoftDeleteFilter<TEntity>(ModelBuilder modelBuilder, ITenantContext tenantContext)
        where TEntity : class, ITenantEntity, ISoftDeletable
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e =>
            !e.IsDeleted &&
            tenantContext.TenantId != null &&
            e.TenantId == tenantContext.TenantId);
    }

    public static void ConfigureMoneyPrecision(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType != typeof(decimal) && property.ClrType != typeof(decimal?))
                {
                    continue;
                }

                var name = property.Name;
                if (name.Contains("ExchangeRate", StringComparison.OrdinalIgnoreCase))
                {
                    property.SetPrecision(19);
                    property.SetScale(8);
                }
                else if (name.Contains("Quantity", StringComparison.OrdinalIgnoreCase) ||
                         name.Contains("Qty", StringComparison.OrdinalIgnoreCase))
                {
                    property.SetPrecision(19);
                    property.SetScale(6);
                }
                else
                {
                    property.SetPrecision(19);
                    property.SetScale(4);
                }
            }
        }
    }
}
""")

write(f"{BASE}/Persistence/AuditableEntityConfiguration.cs", """
using iERP.SharedKernel.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Infrastructure.Persistence;

public abstract class AuditableEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : AuditableEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.TenantId).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).HasDefaultValue(false);
        builder.Property(x => x.Version).IsConcurrencyToken();
        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => new { x.TenantId, x.IsDeleted });
    }
}
""")

write(f"{BASE}/Persistence/Interceptors/TenantSaveChangesInterceptor.cs", """
using iERP.SharedKernel.Primitives;
using iERP.SharedKernel.Tenancy;
using iERP.SharedKernel.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace iERP.Infrastructure.Persistence.Interceptors;

public sealed class TenantSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ITenantContext _tenantContext;
    private readonly IClock _clock;

    public TenantSaveChangesInterceptor(ITenantContext tenantContext, IClock clock)
    {
        _tenantContext = tenantContext;
        _clock = clock;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ApplyRules(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplyRules(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyRules(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        foreach (var entry in context.ChangeTracker.Entries<ITenantEntity>())
        {
            if (entry.State is EntityState.Added)
            {
                if (_tenantContext.HasTenant)
                {
                    if (entry.Entity is TenantEntity tenantEntity && tenantEntity.TenantId == Guid.Empty)
                    {
                        tenantEntity.SetTenantId(_tenantContext.TenantId!.Value);
                    }
                    else if (entry.Entity.TenantId != _tenantContext.TenantId)
                    {
                        throw new InvalidOperationException(
                            $"Entity tenant '{entry.Entity.TenantId}' does not match current tenant '{_tenantContext.TenantId}'.");
                    }
                }
            }

            if (entry.State is EntityState.Modified)
            {
                var original = entry.Property(nameof(ITenantEntity.TenantId)).OriginalValue;
                var current = entry.Property(nameof(ITenantEntity.TenantId)).CurrentValue;
                if (!Equals(original, current))
                {
                    throw new InvalidOperationException("TenantId cannot be changed after insert.");
                }
            }
        }

        foreach (var entry in context.ChangeTracker.Entries<IAuditable>())
        {
            if (entry.State is EntityState.Added)
            {
                entry.Property(nameof(IAuditable.CreatedAt)).CurrentValue = _clock.UtcNow;
            }

            if (entry.State is EntityState.Modified)
            {
                entry.Property(nameof(IAuditable.UpdatedAt)).CurrentValue = _clock.UtcNow;
            }
        }
    }
}
""")

write(f"{BASE}/Persistence/Interceptors/AuditSaveChangesInterceptor.cs", """
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace iERP.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Skeleton interceptor hook for future ActivityLog generation on writes.
/// </summary>
public sealed class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        // Intentionally empty: modules will later project ChangedEntries into ActivityLog.
        _ = eventData.Context?.ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
""")

write(f"{BASE}/Tenancy/ClaimTenantResolver.cs", """
using System.Security.Claims;
using iERP.SharedKernel.Tenancy;
using Microsoft.AspNetCore.Http;

namespace iERP.Infrastructure.Tenancy;

public sealed class ClaimTenantResolver : ITenantResolver
{
    public const string TenantIdClaimType = "tenant_id";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClaimTenantResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<Guid?> ResolveTenantIdAsync(CancellationToken cancellationToken = default)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            return Task.FromResult<Guid?>(null);
        }

        var value = user.FindFirstValue(TenantIdClaimType) ?? user.FindFirstValue("tenantId");
        if (Guid.TryParse(value, out var tenantId))
        {
            return Task.FromResult<Guid?>(tenantId);
        }

        return Task.FromResult<Guid?>(null);
    }
}
""")

write(f"{BASE}/Tenancy/TenantResolutionMiddleware.cs", """
using iERP.SharedKernel.Tenancy;
using Microsoft.AspNetCore.Http;

namespace iERP.Infrastructure.Tenancy;

public sealed class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantResolver resolver, ITenantContext tenantContext)
    {
        var tenantId = await resolver.ResolveTenantIdAsync(context.RequestAborted);
        if (tenantId.HasValue)
        {
            tenantContext.SetTenant(tenantId.Value);
        }

        try
        {
            await _next(context);
        }
        finally
        {
            tenantContext.Clear();
        }
    }
}
""")

write(f"{BASE}/Middleware/CorrelationIdMiddleware.cs", """
using Microsoft.AspNetCore.Http;

namespace iERP.Infrastructure.Middleware;

public sealed class CorrelationIdMiddleware
{
    public const string HeaderName = "X-Correlation-Id";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers.TryGetValue(HeaderName, out var existing) && !string.IsNullOrWhiteSpace(existing)
            ? existing.ToString()
            : Guid.NewGuid().ToString("N");

        context.TraceIdentifier = correlationId;
        context.Response.Headers[HeaderName] = correlationId;
        using (context.RequestServices.GetRequiredService<Microsoft.Extensions.Logging.ILoggerFactory>()
                   .CreateLogger("Correlation")
                   .BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            await _next(context);
        }
    }
}
""")

write(f"{BASE}/Middleware/SecurityHeadersMiddleware.cs", """
using Microsoft.AspNetCore.Http;

namespace iERP.Infrastructure.Middleware;

public sealed class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.Headers.TryAdd("X-Content-Type-Options", "nosniff");
        context.Response.Headers.TryAdd("X-Frame-Options", "DENY");
        context.Response.Headers.TryAdd("Referrer-Policy", "no-referrer");
        context.Response.Headers.TryAdd("X-XSS-Protection", "0");
        await _next(context);
    }
}
""")

write(f"{BASE}/Exceptions/GlobalExceptionHandler.cs", """
using iERP.SharedKernel.Exceptions;
using iERP.SharedKernel.Results;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace iERP.Infrastructure.Exceptions;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, error) = Map(exception);
        if (status >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception");
        }
        else
        {
            _logger.LogWarning(exception, "Handled domain exception {ErrorCode}", error.Error);
        }

        httpContext.Response.StatusCode = status;
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsJsonAsync(error, cancellationToken);
        return true;
    }

    private static (int Status, ApiErrorResponse Error) Map(Exception exception) =>
        exception switch
        {
            ValidationException vex => (
                StatusCodes.Status400BadRequest,
                ApiErrorResponse.Create(vex.ErrorCode, vex.Message, vex.Field, vex.Details)),
            NotFoundException nex => (
                StatusCodes.Status404NotFound,
                ApiErrorResponse.Create(nex.ErrorCode, nex.Message)),
            ForbiddenException fex => (
                StatusCodes.Status403Forbidden,
                ApiErrorResponse.Create(fex.ErrorCode, fex.Message)),
            BusinessRuleException brex => (
                StatusCodes.Status409Conflict,
                ApiErrorResponse.Create(brex.ErrorCode, brex.Message)),
            DomainException dex => (
                StatusCodes.Status400BadRequest,
                ApiErrorResponse.Create(dex.ErrorCode, dex.Message)),
            _ => (
                StatusCodes.Status500InternalServerError,
                ApiErrorResponse.Create(ErrorCodes.InternalError, "An unexpected error occurred."))
        };
}
""")

write(f"{BASE}/Caching/NullCacheService.cs", """
using iERP.Application.Abstractions.Caching;

namespace iERP.Infrastructure.Caching;

public sealed class NullCacheService : ICacheService
{
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) =>
        Task.FromResult<T?>(default);

    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;
}
""")

write(f"{BASE}/Caching/RedisCacheService.cs", """
using System.Text.Json;
using iERP.Application.Abstractions.Caching;
using Microsoft.Extensions.Caching.Distributed;

namespace iERP.Infrastructure.Caching;

public sealed class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var bytes = await _cache.GetAsync(key, cancellationToken);
        if (bytes is null)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(bytes);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(value);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(10)
        };
        await _cache.SetAsync(key, bytes, options, cancellationToken);
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default) =>
        _cache.RemoveAsync(key, cancellationToken);
}
""")

write(f"{BASE}/Messaging/NullEventBus.cs", """
using iERP.Application.Abstractions.Messaging;
using iERP.SharedKernel.Messaging;
using Microsoft.Extensions.Logging;

namespace iERP.Infrastructure.Messaging;

public sealed class NullEventBus : IEventBus, IIntegrationEventPublisher
{
    private readonly ILogger<NullEventBus> _logger;

    public NullEventBus(ILogger<NullEventBus> logger)
    {
        _logger = logger;
    }

    public Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent
    {
        _logger.LogDebug("NullEventBus ignored event {EventType}", integrationEvent.EventType);
        return Task.CompletedTask;
    }

    public Task PublishAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("NullEventBus ignored event {EventType}", integrationEvent.EventType);
        return Task.CompletedTask;
    }
}
""")

write(f"{BASE}/Storage/NullFileStorage.cs", """
using iERP.Application.Abstractions.Storage;
using Microsoft.Extensions.Logging;

namespace iERP.Infrastructure.Storage;

public sealed class NullFileStorage : IFileStorage
{
    private readonly ILogger<NullFileStorage> _logger;

    public NullFileStorage(ILogger<NullFileStorage> logger)
    {
        _logger = logger;
    }

    public Task<string> UploadAsync(Stream content, string blobPath, string contentType, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("NullFileStorage upload stub for {BlobPath}", blobPath);
        return Task.FromResult(blobPath);
    }

    public Task<Stream> DownloadAsync(string blobPath, CancellationToken cancellationToken = default) =>
        Task.FromResult<Stream>(new MemoryStream());

    public Task DeleteAsync(string blobPath, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;
}
""")

write(f"{BASE}/Storage/AzureBlobFileStorage.cs", """
using Azure.Storage.Blobs;
using iERP.Application.Abstractions.Options;
using iERP.Application.Abstractions.Storage;
using Microsoft.Extensions.Options;

namespace iERP.Infrastructure.Storage;

public sealed class AzureBlobFileStorage : IFileStorage
{
    private readonly BlobContainerClient _container;

    public AzureBlobFileStorage(IOptions<AzureBlobStorageOptions> options)
    {
        var value = options.Value;
        var service = new BlobServiceClient(value.ConnectionString);
        _container = service.GetBlobContainerClient(value.ContainerName);
    }

    public async Task<string> UploadAsync(Stream content, string blobPath, string contentType, CancellationToken cancellationToken = default)
    {
        var client = _container.GetBlobClient(blobPath);
        await client.UploadAsync(content, overwrite: true, cancellationToken);
        return blobPath;
    }

    public async Task<Stream> DownloadAsync(string blobPath, CancellationToken cancellationToken = default)
    {
        var client = _container.GetBlobClient(blobPath);
        var response = await client.DownloadStreamingAsync(cancellationToken: cancellationToken);
        return response.Value.Content;
    }

    public async Task DeleteAsync(string blobPath, CancellationToken cancellationToken = default)
    {
        var client = _container.GetBlobClient(blobPath);
        await client.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }
}
""")

write(f"{BASE}/AI/NullLLMProvider.cs", """
using iERP.Application.Abstractions.AI;

namespace iERP.Infrastructure.AI;

public sealed class NullLLMProvider : ILLMProvider
{
    public Task<string> CompleteAsync(string prompt, CancellationToken cancellationToken = default) =>
        Task.FromResult("AI provider is not configured.");
}
""")

write(f"{BASE}/AI/AzureOpenAIProvider.cs", """
using iERP.Application.Abstractions.AI;
using iERP.Application.Abstractions.Options;
using Microsoft.Extensions.Options;

namespace iERP.Infrastructure.AI;

/// <summary>
/// Placeholder Azure OpenAI provider. Real Semantic Kernel wiring lives in the AI module.
/// </summary>
public sealed class AzureOpenAIProvider : ILLMProvider
{
    private readonly AzureOpenAIOptions _options;

    public AzureOpenAIProvider(IOptions<AzureOpenAIOptions> options)
    {
        _options = options.Value;
    }

    public Task<string> CompleteAsync(string prompt, CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled || string.IsNullOrWhiteSpace(_options.Endpoint))
        {
            return Task.FromResult("Azure OpenAI is not enabled.");
        }

        // Intentionally not calling Azure here so local startup never requires credentials.
        return Task.FromResult($"[AzureOpenAI:{_options.DeploymentName}] prompt received ({prompt.Length} chars)");
    }
}
""")

write(f"{BASE}/Notifications/NullEmailSender.cs", """
using iERP.Application.Abstractions.Notifications;
using Microsoft.Extensions.Logging;

namespace iERP.Infrastructure.Notifications;

public sealed class NullEmailSender : IEmailSender
{
    private readonly ILogger<NullEmailSender> _logger;

    public NullEmailSender(ILogger<NullEmailSender> logger) => _logger = logger;

    public Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("NullEmailSender to {To}: {Subject}", to, subject);
        return Task.CompletedTask;
    }
}
""")

write(f"{BASE}/Notifications/NullWhatsAppSender.cs", """
using iERP.Application.Abstractions.Notifications;
using Microsoft.Extensions.Logging;

namespace iERP.Infrastructure.Notifications;

public sealed class NullWhatsAppSender : IWhatsAppSender
{
    private readonly ILogger<NullWhatsAppSender> _logger;

    public NullWhatsAppSender(ILogger<NullWhatsAppSender> logger) => _logger = logger;

    public Task SendAsync(string toPhoneNumber, string message, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("NullWhatsAppSender to {To}", toPhoneNumber);
        return Task.CompletedTask;
    }
}
""")

write(f"{BASE}/Notifications/NullNotificationService.cs", """
using iERP.Application.Abstractions.Notifications;
using Microsoft.Extensions.Logging;

namespace iERP.Infrastructure.Notifications;

public sealed class NullNotificationService : INotificationService
{
    private readonly ILogger<NullNotificationService> _logger;

    public NullNotificationService(ILogger<NullNotificationService> logger) => _logger = logger;

    public Task NotifyAsync(
        Guid tenantId,
        Guid? userId,
        string channel,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("NullNotificationService {Channel} tenant {TenantId}", channel, tenantId);
        return Task.CompletedTask;
    }
}
""")

write(f"{BASE}/Jobs/HangfireBackgroundJobService.cs", """
using Hangfire;
using iERP.Application.Abstractions.Jobs;
using Microsoft.Extensions.Logging;

namespace iERP.Infrastructure.Jobs;

public sealed class HangfireBackgroundJobService : IBackgroundJobService
{
    private readonly ILogger<HangfireBackgroundJobService> _logger;

    public HangfireBackgroundJobService(ILogger<HangfireBackgroundJobService> logger)
    {
        _logger = logger;
    }

    public string Enqueue(ExpressionJob job)
    {
        _logger.LogInformation("Enqueue placeholder job {JobName}", job.JobName);
        return BackgroundJob.Enqueue(() => ExecutePlaceholder(job.JobName));
    }

    public string Schedule(ExpressionJob job, TimeSpan delay)
    {
        _logger.LogInformation("Schedule placeholder job {JobName} in {Delay}", job.JobName, delay);
        return BackgroundJob.Schedule(() => ExecutePlaceholder(job.JobName), delay);
    }

    public static void ExecutePlaceholder(string jobName)
    {
        // No-op placeholder for foundation.
    }
}
""")

write(f"{BASE}/Reporting/ReportingDbConnectionFactory.cs", """
using System.Data.Common;
using iERP.Application.Abstractions.Options;
using iERP.Application.Abstractions.Reporting;
using Microsoft.Extensions.Options;
using Npgsql;

namespace iERP.Infrastructure.Reporting;

public sealed class ReportingDbConnectionFactory : IReportingDbConnectionFactory
{
    private readonly DatabaseOptions _options;

    public ReportingDbConnectionFactory(IOptions<DatabaseOptions> options)
    {
        _options = options.Value;
    }

    public async Task<DbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connectionString = string.IsNullOrWhiteSpace(_options.ReportingDatabase)
            ? _options.PrimaryDatabase
            : _options.ReportingDatabase;

        var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
""")

write(f"{BASE}/DependencyInjection.cs", """
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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Text;
using System.Threading.RateLimiting;

namespace iERP.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIerpInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));
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
        var bus = configuration.GetSection(AzureServiceBusOptions.SectionName).Get<AzureServiceBusOptions>() ?? new AzureServiceBusOptions();
        services.AddSingleton<IEventBus, NullEventBus>();
        services.AddSingleton<IIntegrationEventPublisher, NullEventBus>();

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

        _ = bus; // reserved for future Azure Service Bus publisher registration
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
        var db = configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>() ?? new DatabaseOptions();
        var redis = configuration.GetSection(RedisOptions.SectionName).Get<RedisOptions>() ?? new RedisOptions();

        var builder = services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"]);

        if (!string.IsNullOrWhiteSpace(db.PrimaryDatabase))
        {
            builder.AddNpgSql(db.PrimaryDatabase, name: "postgres", tags: ["ready"]);
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

# Fix missing using in DependencyInjection
# Need Microsoft.AspNetCore.Http for StatusCodes and HttpContext
print("infra done")
