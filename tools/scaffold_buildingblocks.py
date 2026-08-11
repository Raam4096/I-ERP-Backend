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

# ===================== SharedKernel =====================

write("src/BuildingBlocks/iERP.SharedKernel/Primitives/IDomainEvent.cs", """
namespace iERP.SharedKernel.Primitives;

public interface IDomainEvent
{
    Guid EventId { get; }
    DateTimeOffset OccurredAt { get; }
}
""")

write("src/BuildingBlocks/iERP.SharedKernel/Primitives/DomainEvent.cs", """
namespace iERP.SharedKernel.Primitives;

public abstract record DomainEvent : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;
}
""")

write("src/BuildingBlocks/iERP.SharedKernel/Primitives/ITenantEntity.cs", """
namespace iERP.SharedKernel.Primitives;

public interface ITenantEntity
{
    Guid TenantId { get; }
}
""")

write("src/BuildingBlocks/iERP.SharedKernel/Primitives/ISoftDeletable.cs", """
namespace iERP.SharedKernel.Primitives;

public interface ISoftDeletable
{
    bool IsDeleted { get; }
    DateTimeOffset? DeletedAt { get; }
    Guid? DeletedBy { get; }
}
""")

write("src/BuildingBlocks/iERP.SharedKernel/Primitives/IAuditable.cs", """
namespace iERP.SharedKernel.Primitives;

public interface IAuditable
{
    DateTimeOffset CreatedAt { get; }
    Guid? CreatedBy { get; }
    DateTimeOffset? UpdatedAt { get; }
    Guid? UpdatedBy { get; }
}
""")

write("src/BuildingBlocks/iERP.SharedKernel/Primitives/Entity.cs", """
namespace iERP.SharedKernel.Primitives;

public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public Guid Id { get; protected set; } = Guid.NewGuid();

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void Raise(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
""")

write("src/BuildingBlocks/iERP.SharedKernel/Primitives/TenantEntity.cs", """
namespace iERP.SharedKernel.Primitives;

public abstract class TenantEntity : Entity, ITenantEntity
{
    public Guid TenantId { get; protected set; }

    protected TenantEntity()
    {
    }

    protected TenantEntity(Guid tenantId)
    {
        TenantId = tenantId;
    }

    public void SetTenantId(Guid tenantId)
    {
        if (TenantId != Guid.Empty && TenantId != tenantId)
        {
            throw new InvalidOperationException("TenantId cannot be changed after it has been assigned.");
        }

        TenantId = tenantId;
    }
}
""")

write("src/BuildingBlocks/iERP.SharedKernel/Primitives/AuditableEntity.cs", """
namespace iERP.SharedKernel.Primitives;

public abstract class AuditableEntity : TenantEntity, IAuditable, ISoftDeletable
{
    public DateTimeOffset CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
    public long Version { get; set; }

    protected AuditableEntity()
    {
    }

    protected AuditableEntity(Guid tenantId) : base(tenantId)
    {
    }

    public void SoftDelete(Guid? deletedBy, DateTimeOffset deletedAt)
    {
        IsDeleted = true;
        DeletedBy = deletedBy;
        DeletedAt = deletedAt;
    }
}
""")

write("src/BuildingBlocks/iERP.SharedKernel/Time/IClock.cs", """
namespace iERP.SharedKernel.Time;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
""")

write("src/BuildingBlocks/iERP.SharedKernel/Time/SystemClock.cs", """
namespace iERP.SharedKernel.Time;

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
""")

write("src/BuildingBlocks/iERP.SharedKernel/Tenancy/ITenantContext.cs", """
namespace iERP.SharedKernel.Tenancy;

public interface ITenantContext
{
    Guid? TenantId { get; }
    bool HasTenant { get; }
    void SetTenant(Guid tenantId);
    void Clear();
}
""")

write("src/BuildingBlocks/iERP.SharedKernel/Tenancy/TenantContext.cs", """
namespace iERP.SharedKernel.Tenancy;

public sealed class TenantContext : ITenantContext
{
    public Guid? TenantId { get; private set; }
    public bool HasTenant => TenantId.HasValue && TenantId.Value != Guid.Empty;

    public void SetTenant(Guid tenantId)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("TenantId cannot be empty.", nameof(tenantId));
        }

        TenantId = tenantId;
    }

    public void Clear() => TenantId = null;
}
""")

write("src/BuildingBlocks/iERP.SharedKernel/Tenancy/ITenantResolver.cs", """
namespace iERP.SharedKernel.Tenancy;

public interface ITenantResolver
{
    Task<Guid?> ResolveTenantIdAsync(CancellationToken cancellationToken = default);
}
""")

write("src/BuildingBlocks/iERP.SharedKernel/Exceptions/DomainException.cs", """
namespace iERP.SharedKernel.Exceptions;

public class DomainException : Exception
{
    public string ErrorCode { get; }

    public DomainException(string errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }
}
""")

write("src/BuildingBlocks/iERP.SharedKernel/Exceptions/ValidationException.cs", """
namespace iERP.SharedKernel.Exceptions;

public sealed class ValidationException : DomainException
{
    public string? Field { get; }
    public IReadOnlyList<string> Details { get; }

    public ValidationException(string message, string? field = null, IEnumerable<string>? details = null)
        : base(ErrorCodes.ValidationError, message)
    {
        Field = field;
        Details = details?.ToArray() ?? [];
    }
}
""")

write("src/BuildingBlocks/iERP.SharedKernel/Exceptions/NotFoundException.cs", """
namespace iERP.SharedKernel.Exceptions;

public sealed class NotFoundException : DomainException
{
    public NotFoundException(string message)
        : base(ErrorCodes.NotFound, message)
    {
    }
}
""")

write("src/BuildingBlocks/iERP.SharedKernel/Exceptions/ForbiddenException.cs", """
namespace iERP.SharedKernel.Exceptions;

public sealed class ForbiddenException : DomainException
{
    public ForbiddenException(string message, string errorCode = ErrorCodes.Forbidden)
        : base(errorCode, message)
    {
    }
}
""")

write("src/BuildingBlocks/iERP.SharedKernel/Exceptions/BusinessRuleException.cs", """
namespace iERP.SharedKernel.Exceptions;

public sealed class BusinessRuleException : DomainException
{
    public BusinessRuleException(string errorCode, string message)
        : base(errorCode, message)
    {
    }
}
""")

write("src/BuildingBlocks/iERP.SharedKernel/Exceptions/ErrorCodes.cs", """
namespace iERP.SharedKernel.Exceptions;

public static class ErrorCodes
{
    public const string ValidationError = "VALIDATION_ERROR";
    public const string DuplicateRecord = "DUPLICATE_RECORD";
    public const string BusinessRuleViolation = "BUSINESS_RULE_VIOLATION";
    public const string InvalidStatusTransition = "INVALID_STATUS_TRANSITION";
    public const string DocumentAlreadyPosted = "DOCUMENT_ALREADY_POSTED";
    public const string CreditLimitExceeded = "CREDIT_LIMIT_EXCEEDED";
    public const string InsufficientStock = "INSUFFICIENT_STOCK";

    public const string Unauthorized = "UNAUTHORIZED";
    public const string TokenExpired = "TOKEN_EXPIRED";
    public const string Forbidden = "FORBIDDEN";
    public const string TenantNotFound = "TENANT_NOT_FOUND";
    public const string TenantSuspended = "TENANT_SUSPENDED";
    public const string FieldPermissionDenied = "FIELD_PERMISSION_DENIED";

    public const string AiPermissionDenied = "AI_PERMISSION_DENIED";
    public const string AiApprovalRequired = "AI_APPROVAL_REQUIRED";
    public const string AiRollbackFailed = "AI_ROLLBACK_FAILED";

    public const string WorkflowError = "WORKFLOW_ERROR";
    public const string BridgeConditionNotMet = "BRIDGE_CONDITION_NOT_MET";
    public const string NotFound = "NOT_FOUND";
    public const string InternalError = "INTERNAL_ERROR";
}
""")

write("src/BuildingBlocks/iERP.SharedKernel/Results/ApiResponse.cs", """
namespace iERP.SharedKernel.Results;

public sealed record ApiResponse<T>(
    bool Success,
    T? Data,
    string? Message = null)
{
    public static ApiResponse<T> Ok(T data, string? message = null) =>
        new(true, data, message ?? "Operation successful");

    public static ApiResponse<T> Fail(string message) =>
        new(false, default, message);
}
""")

write("src/BuildingBlocks/iERP.SharedKernel/Results/ApiErrorResponse.cs", """
namespace iERP.SharedKernel.Results;

public sealed record ApiErrorResponse(
    bool Success,
    string Error,
    string Message,
    string? Field = null,
    IReadOnlyList<string>? Details = null)
{
    public static ApiErrorResponse Create(
        string error,
        string message,
        string? field = null,
        IReadOnlyList<string>? details = null) =>
        new(false, error, message, field, details);
}
""")

write("src/BuildingBlocks/iERP.SharedKernel/Results/PaginationMetadata.cs", """
namespace iERP.SharedKernel.Results;

public sealed record PaginationMetadata(
    int Page,
    int PageSize,
    long TotalCount,
    int TotalPages)
{
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
}
""")

write("src/BuildingBlocks/iERP.SharedKernel/Results/PagedResponse.cs", """
namespace iERP.SharedKernel.Results;

public sealed record PagedResponse<T>(
    bool Success,
    IReadOnlyList<T> Data,
    PaginationMetadata Pagination,
    string? Message = null)
{
    public static PagedResponse<T> Create(
        IReadOnlyList<T> data,
        int page,
        int pageSize,
        long totalCount,
        string? message = null)
    {
        var totalPages = pageSize <= 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);
        return new(
            true,
            data,
            new PaginationMetadata(page, pageSize, totalCount, totalPages),
            message);
    }
}
""")

write("src/BuildingBlocks/iERP.SharedKernel/Results/PaginationDefaults.cs", """
namespace iERP.SharedKernel.Results;

public static class PaginationDefaults
{
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;
}
""")

write("src/BuildingBlocks/iERP.SharedKernel/Security/Permissions.cs", """
namespace iERP.SharedKernel.Security;

/// <summary>
/// Permission constants use module.resource.action naming.
/// </summary>
public static class Permissions
{
    public static class Crm
    {
        public const string LeadRead = "crm.lead.read";
        public const string LeadCreate = "crm.lead.create";
        public const string LeadUpdate = "crm.lead.update";
        public const string OpportunityRead = "crm.opportunity.read";
        public const string OpportunityCreate = "crm.opportunity.create";
        public const string CustomerRead = "crm.customer.read";
        public const string CustomerCreate = "crm.customer.create";
        public const string CustomerUpdate = "crm.customer.update";
    }

    public static class Sales
    {
        public const string QuotationRead = "sales.quotation.read";
        public const string QuotationCreate = "sales.quotation.create";
        public const string QuotationApprove = "sales.quotation.approve";
        public const string OrderCreate = "sales.order.create";
        public const string OrderRead = "sales.order.read";
        public const string InvoiceRead = "sales.invoice.read";
        public const string InvoiceCreate = "sales.invoice.create";
    }

    public static class Finance
    {
        public const string InvoiceApprove = "finance.invoice.approve";
        public const string GlPost = "finance.gl.post";
        public const string JournalRead = "finance.journal.read";
        public const string JournalCreate = "finance.journal.create";
    }

    public static class Procurement
    {
        public const string PurchaseOrderRead = "procurement.purchase_order.read";
        public const string PurchaseOrderCreate = "procurement.purchase_order.create";
        public const string VendorRead = "procurement.vendor.read";
        public const string VendorCreate = "procurement.vendor.create";
    }

    public static class Inventory
    {
        public const string StockRead = "inventory.stock.read";
        public const string StockAdjust = "inventory.stock.adjust";
        public const string TransferCreate = "inventory.transfer.create";
    }

    public static class Catalog
    {
        public const string ItemRead = "catalog.item.read";
        public const string ItemCreate = "catalog.item.create";
        public const string ItemUpdate = "catalog.item.update";
    }

    public static class Ai
    {
        public const string ExecuteAdvisory = "ai.tool.execute.advisory";
        public const string ExecuteSemiAutonomous = "ai.tool.execute.semi_autonomous";
        public const string ExecuteAutonomous = "ai.tool.execute.autonomous";
    }

    public static class Platform
    {
        public const string TenantManage = "platform.tenant.manage";
        public const string UserManage = "platform.user.manage";
        public const string RoleManage = "platform.role.manage";
        public const string MetadataManage = "platform.metadata.manage";
    }
}
""")

write("src/BuildingBlocks/iERP.SharedKernel/Security/FieldPermission.cs", """
namespace iERP.SharedKernel.Security;

/// <summary>
/// Describes field-level permission for an entity field within a tenant/role context.
/// </summary>
public sealed class FieldPermission
{
    public required string EntityName { get; init; }
    public required string FieldKey { get; init; }
    public bool CanView { get; init; } = true;
    public bool CanEdit { get; init; }
}
""")

write("src/BuildingBlocks/iERP.SharedKernel/Security/SystemRoles.cs", """
namespace iERP.SharedKernel.Security;

public static class SystemRoles
{
    public const string SuperAdmin = "Super Admin";
    public const string TenantAdmin = "Tenant Admin";
    public const string FinanceManager = "Finance Manager";
    public const string FinanceExecutive = "Finance Executive";
    public const string SalesManager = "Sales Manager";
    public const string SalesExecutive = "Sales Executive";
    public const string PurchaseManager = "Purchase Manager";
    public const string PurchaseExecutive = "Purchase Executive";
    public const string WarehouseStaff = "Warehouse Staff";
    public const string ReadOnly = "Read Only";

    public static IReadOnlyList<string> All { get; } =
    [
        SuperAdmin,
        TenantAdmin,
        FinanceManager,
        FinanceExecutive,
        SalesManager,
        SalesExecutive,
        PurchaseManager,
        PurchaseExecutive,
        WarehouseStaff,
        ReadOnly
    ];
}
""")

write("src/BuildingBlocks/iERP.SharedKernel/Messaging/IIntegrationEvent.cs", """
namespace iERP.SharedKernel.Messaging;

public interface IIntegrationEvent
{
    Guid EventId { get; }
    Guid? TenantId { get; }
    string EventType { get; }
    DateTimeOffset OccurredAt { get; }
}
""")

write("src/BuildingBlocks/iERP.SharedKernel/Messaging/IntegrationEvent.cs", """
namespace iERP.SharedKernel.Messaging;

public abstract record IntegrationEvent : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public Guid? TenantId { get; init; }
    public abstract string EventType { get; }
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;
}
""")

# ===================== Application.Abstractions =====================

write("src/BuildingBlocks/iERP.Application.Abstractions/Caching/ICacheService.cs", """
namespace iERP.Application.Abstractions.Caching;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    static string BuildKey(Guid tenantId, string module, string resource, string key) =>
        $"ierp:{tenantId}:{module}:{resource}:{key}";
}
""")

write("src/BuildingBlocks/iERP.Application.Abstractions/Messaging/IEventBus.cs", """
using iERP.SharedKernel.Messaging;

namespace iERP.Application.Abstractions.Messaging;

public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent;
}
""")

write("src/BuildingBlocks/iERP.Application.Abstractions/Messaging/IIntegrationEventPublisher.cs", """
using iERP.SharedKernel.Messaging;

namespace iERP.Application.Abstractions.Messaging;

public interface IIntegrationEventPublisher
{
    Task PublishAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default);
}
""")

write("src/BuildingBlocks/iERP.Application.Abstractions/Storage/IFileStorage.cs", """
namespace iERP.Application.Abstractions.Storage;

public interface IFileStorage
{
    Task<string> UploadAsync(
        Stream content,
        string blobPath,
        string contentType,
        CancellationToken cancellationToken = default);

    Task<Stream> DownloadAsync(string blobPath, CancellationToken cancellationToken = default);

    Task DeleteAsync(string blobPath, CancellationToken cancellationToken = default);
}
""")

write("src/BuildingBlocks/iERP.Application.Abstractions/Notifications/IEmailSender.cs", """
namespace iERP.Application.Abstractions.Notifications;

public interface IEmailSender
{
    Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
}
""")

write("src/BuildingBlocks/iERP.Application.Abstractions/Notifications/IWhatsAppSender.cs", """
namespace iERP.Application.Abstractions.Notifications;

public interface IWhatsAppSender
{
    Task SendAsync(string toPhoneNumber, string message, CancellationToken cancellationToken = default);
}
""")

write("src/BuildingBlocks/iERP.Application.Abstractions/Notifications/INotificationService.cs", """
namespace iERP.Application.Abstractions.Notifications;

public interface INotificationService
{
    Task NotifyAsync(
        Guid tenantId,
        Guid? userId,
        string channel,
        string subject,
        string body,
        CancellationToken cancellationToken = default);
}
""")

write("src/BuildingBlocks/iERP.Application.Abstractions/Jobs/IBackgroundJobService.cs", """
namespace iERP.Application.Abstractions.Jobs;

public interface IBackgroundJobService
{
    string Enqueue(ExpressionJob job);
    string Schedule(ExpressionJob job, TimeSpan delay);
}

/// <summary>
/// Lightweight job descriptor placeholder until Hangfire expressions are wired per module.
/// </summary>
public sealed record ExpressionJob(string JobName, IDictionary<string, string>? Parameters = null);
""")

write("src/BuildingBlocks/iERP.Application.Abstractions/AI/ILLMProvider.cs", """
namespace iERP.Application.Abstractions.AI;

public interface ILLMProvider
{
    Task<string> CompleteAsync(string prompt, CancellationToken cancellationToken = default);
}
""")

write("src/BuildingBlocks/iERP.Application.Abstractions/AI/IAIOrchestrator.cs", """
namespace iERP.Application.Abstractions.AI;

public interface IAIOrchestrator
{
    Task<AIOrchestrationResult> ExecuteAsync(AIOrchestrationRequest request, CancellationToken cancellationToken = default);
}

public sealed record AIOrchestrationRequest(
    Guid TenantId,
    Guid UserId,
    string Prompt,
    string? ToolName = null,
    string ExecutionMode = "advisory");

public sealed record AIOrchestrationResult(
    bool Success,
    string? Response,
    string Status,
    string? Error = null);
""")

write("src/BuildingBlocks/iERP.Application.Abstractions/AI/IAITool.cs", """
namespace iERP.Application.Abstractions.AI;

public interface IAITool
{
    string Name { get; }
    string Description { get; }
    Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default);
}

public sealed record AIToolContext(
    Guid TenantId,
    Guid UserId,
    string PayloadJson,
    string ExecutionMode);

public sealed record AIToolResult(bool Success, string? ResultJson, string? Error = null);
""")

write("src/BuildingBlocks/iERP.Application.Abstractions/AI/IAIToolRegistry.cs", """
namespace iERP.Application.Abstractions.AI;

public interface IAIToolRegistry
{
    IAITool? Resolve(string toolName);
    IReadOnlyCollection<IAITool> GetAll();
    void Register(IAITool tool);
}
""")

write("src/BuildingBlocks/iERP.Application.Abstractions/AI/IAIGovernanceService.cs", """
namespace iERP.Application.Abstractions.AI;

public interface IAIGovernanceService
{
    Task<AIGovernanceDecision> AuthorizeAsync(
        Guid tenantId,
        Guid userId,
        string toolName,
        string executionMode,
        CancellationToken cancellationToken = default);
}

public sealed record AIGovernanceDecision(bool Allowed, string? Reason = null, bool RequiresApproval = false);
""")

write("src/BuildingBlocks/iERP.Application.Abstractions/Engines/IWorkflowEngine.cs", """
namespace iERP.Application.Abstractions.Engines;

public interface IWorkflowEngine
{
    Task StartAsync(Guid tenantId, string entityName, Guid recordId, Guid workflowId, Guid startedBy, CancellationToken cancellationToken = default);
    Task AdvanceAsync(Guid tenantId, Guid instanceId, string action, Guid actedBy, CancellationToken cancellationToken = default);
    Task CancelAsync(Guid tenantId, Guid instanceId, Guid actedBy, string? reason = null, CancellationToken cancellationToken = default);
}
""")

write("src/BuildingBlocks/iERP.Application.Abstractions/Engines/IRuleEngine.cs", """
namespace iERP.Application.Abstractions.Engines;

public interface IRuleEngine
{
    Task EvaluateAsync(Guid tenantId, string entityName, string eventName, object context, CancellationToken cancellationToken = default);
}
""")

write("src/BuildingBlocks/iERP.Application.Abstractions/Engines/IBridgeEngine.cs", """
namespace iERP.Application.Abstractions.Engines;

public interface IBridgeEngine
{
    Task ConvertAsync(Guid tenantId, Guid bridgeDefinitionId, Guid sourceRecordId, Guid actedBy, CancellationToken cancellationToken = default);
}
""")

write("src/BuildingBlocks/iERP.Application.Abstractions/Engines/IPrintEngine.cs", """
namespace iERP.Application.Abstractions.Engines;

public interface IPrintEngine
{
    Task<byte[]> RenderAsync(Guid tenantId, string entityName, Guid recordId, string templateCode, CancellationToken cancellationToken = default);
}
""")

write("src/BuildingBlocks/iERP.Application.Abstractions/Reporting/IReportingDbConnectionFactory.cs", """
using System.Data.Common;

namespace iERP.Application.Abstractions.Reporting;

public interface IReportingDbConnectionFactory
{
    Task<DbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default);
}
""")

write("src/BuildingBlocks/iERP.Application.Abstractions/Seeding/IDataSeeder.cs", """
namespace iERP.Application.Abstractions.Seeding;

public interface IDataSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
""")

write("src/BuildingBlocks/iERP.Application.Abstractions/Common/ICommandHandler.cs", """
namespace iERP.Application.Abstractions.Common;

/// <summary>
/// Lightweight command handler abstraction. Can later be adapted to MediatR.
/// </summary>
public interface ICommandHandler<in TCommand, TResult>
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

public interface IQueryHandler<in TQuery, TResult>
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
""")

write("src/BuildingBlocks/iERP.Application.Abstractions/Options/DatabaseOptions.cs", """
namespace iERP.Application.Abstractions.Options;

public sealed class DatabaseOptions
{
    public const string SectionName = "ConnectionStrings";

    public string PrimaryDatabase { get; set; } = string.Empty;
    public string ReportingDatabase { get; set; } = string.Empty;
}
""")

write("src/BuildingBlocks/iERP.Application.Abstractions/Options/JwtOptions.cs", """
namespace iERP.Application.Abstractions.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "i-ERP";
    public string Audience { get; set; } = "i-ERP";
    public string SigningKey { get; set; } = string.Empty;
    public int AccessTokenMinutes { get; set; } = 30;
    public int RefreshTokenDays { get; set; } = 14;
}
""")

write("src/BuildingBlocks/iERP.Application.Abstractions/Options/RedisOptions.cs", """
namespace iERP.Application.Abstractions.Options;

public sealed class RedisOptions
{
    public const string SectionName = "Redis";

    public bool Enabled { get; set; }
    public string ConnectionString { get; set; } = "localhost:6379";
    public string InstanceName { get; set; } = "ierp:";
}
""")

write("src/BuildingBlocks/iERP.Application.Abstractions/Options/AzureServiceBusOptions.cs", """
namespace iERP.Application.Abstractions.Options;

public sealed class AzureServiceBusOptions
{
    public const string SectionName = "AzureServiceBus";

    public bool Enabled { get; set; }
    public string ConnectionString { get; set; } = string.Empty;
    public string TopicName { get; set; } = "ierp-events";
}
""")

write("src/BuildingBlocks/iERP.Application.Abstractions/Options/AzureOpenAIOptions.cs", """
namespace iERP.Application.Abstractions.Options;

public sealed class AzureOpenAIOptions
{
    public const string SectionName = "AzureOpenAI";

    public bool Enabled { get; set; }
    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string DeploymentName { get; set; } = string.Empty;
}
""")

write("src/BuildingBlocks/iERP.Application.Abstractions/Options/AzureBlobStorageOptions.cs", """
namespace iERP.Application.Abstractions.Options;

public sealed class AzureBlobStorageOptions
{
    public const string SectionName = "AzureBlobStorage";

    public bool Enabled { get; set; }
    public string ConnectionString { get; set; } = string.Empty;
    public string ContainerName { get; set; } = "ierp-attachments";
}
""")

write("src/BuildingBlocks/iERP.Application.Abstractions/Options/HangfireOptions.cs", """
namespace iERP.Application.Abstractions.Options;

public sealed class HangfireOptions
{
    public const string SectionName = "Hangfire";

    public bool Enabled { get; set; } = true;
    public string SchemaName { get; set; } = "hangfire";
    public int WorkerCount { get; set; } = 2;
}
""")

print("building blocks contracts done")
