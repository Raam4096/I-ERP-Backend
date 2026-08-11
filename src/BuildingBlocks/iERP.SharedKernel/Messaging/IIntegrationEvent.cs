namespace iERP.SharedKernel.Messaging;

public interface IIntegrationEvent
{
    Guid EventId { get; }
    Guid? TenantId { get; }
    string EventType { get; }
    DateTimeOffset OccurredAt { get; }
}
