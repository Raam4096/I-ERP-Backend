using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Engines.Workflow.Domain;

public sealed class WorkflowHistory : AuditableEntity
{

    public Guid WorkflowInstanceId { get; set; }
    public string StepCode { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public Guid ActedBy { get; set; }
    public string? Comments { get; set; }

}
