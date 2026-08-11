using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Engines.Workflow.Domain;

public sealed class WorkflowInstance : AuditableEntity
{

    public string EntityName { get; set; } = string.Empty;
    public Guid RecordId { get; set; }
    public Guid WorkflowId { get; set; }
    public string CurrentStep { get; set; } = string.Empty;
    public string Status { get; set; } = "active";
    public Guid StartedBy { get; set; }
    public string? RejectionReason { get; set; }
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }

}
