using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Platform.Tenancy.Domain;

public sealed class OutboxMessage : Entity
{
    public Guid? TenantId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Payload { get; set; } = "{}";
    public DateTimeOffset OccurredAt { get; set; }
    public DateTimeOffset? ProcessedAt { get; set; }
    public int RetryCount { get; set; }
    public string? Error { get; set; }
}
