using iERP.Application.Abstractions.AI;

namespace iERP.Modules.AI.Application;

public sealed class NullAIGovernanceService : IAIGovernanceService
{
    public Task<AIGovernanceDecision> AuthorizeAsync(
        Guid tenantId, Guid userId, string toolName, string executionMode, CancellationToken cancellationToken = default)
        => Task.FromResult(new AIGovernanceDecision(false, "AI governance not configured."));
}
