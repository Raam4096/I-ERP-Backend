using iERP.Application.Abstractions.AI;

namespace iERP.Modules.AI.Application;

public sealed class NullAIOrchestrator : IAIOrchestrator
{
    public Task<AIOrchestrationResult> ExecuteAsync(AIOrchestrationRequest request, CancellationToken cancellationToken = default)
        => Task.FromResult(new AIOrchestrationResult(false, null, "not_implemented", "AI orchestrator placeholder."));
}
