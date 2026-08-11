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
