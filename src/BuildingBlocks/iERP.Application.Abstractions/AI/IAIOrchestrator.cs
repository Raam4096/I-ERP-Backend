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
