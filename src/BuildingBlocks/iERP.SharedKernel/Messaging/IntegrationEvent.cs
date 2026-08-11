namespace iERP.SharedKernel.Messaging;

public abstract record IntegrationEvent : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public Guid? TenantId { get; init; }
    public abstract string EventType { get; }
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;
}
