using iERP.Application.Abstractions.Engines;
using Microsoft.Extensions.Logging;

namespace iERP.Modules.Engines.Workflow.Application;

public sealed class NullWorkflowEngine : IWorkflowEngine
{
    private readonly ILogger<NullWorkflowEngine> _logger;
    public NullWorkflowEngine(ILogger<NullWorkflowEngine> logger) => _logger = logger;
    public Task StartAsync(Guid tenantId, string entityName, Guid recordId, Guid workflowId, Guid startedBy, CancellationToken cancellationToken = default)
    { _logger.LogDebug("Workflow start placeholder"); return Task.CompletedTask; }
    public Task AdvanceAsync(Guid tenantId, Guid instanceId, string action, Guid actedBy, CancellationToken cancellationToken = default)
    { return Task.CompletedTask; }
    public Task CancelAsync(Guid tenantId, Guid instanceId, Guid actedBy, string? reason = null, CancellationToken cancellationToken = default)
    { return Task.CompletedTask; }
}
