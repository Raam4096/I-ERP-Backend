using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Engines.Workflow.Domain;

public sealed class WorkflowStep : AuditableEntity
{

    public Guid WorkflowDefinitionId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int StepOrder { get; set; }
    public string? ApproverRole { get; set; }

}
