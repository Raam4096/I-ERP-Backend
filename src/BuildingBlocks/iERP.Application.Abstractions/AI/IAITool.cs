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
