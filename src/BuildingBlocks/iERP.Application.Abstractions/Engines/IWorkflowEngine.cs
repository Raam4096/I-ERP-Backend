namespace iERP.Application.Abstractions.Engines;

public interface IWorkflowEngine
{
    Task StartAsync(Guid tenantId, string entityName, Guid recordId, Guid workflowId, Guid startedBy, CancellationToken cancellationToken = default);
    Task AdvanceAsync(Guid tenantId, Guid instanceId, string action, Guid actedBy, CancellationToken cancellationToken = default);
    Task CancelAsync(Guid tenantId, Guid instanceId, Guid actedBy, string? reason = null, CancellationToken cancellationToken = default);
}
