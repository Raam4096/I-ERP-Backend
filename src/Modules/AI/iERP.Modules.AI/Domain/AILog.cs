using iERP.SharedKernel.Primitives;

namespace iERP.Modules.AI.Domain;

public sealed class AILog : AuditableEntity
{

    public Guid UserId { get; set; }
    public string ToolName { get; set; } = string.Empty;
    public string Prompt { get; set; } = string.Empty;
    public string? Response { get; set; }
    public string? ActionType { get; set; }
    public string ExecutionMode { get; set; } = "advisory";
    public string Status { get; set; } = "completed";
    public string? RollbackPayload { get; set; }

}
