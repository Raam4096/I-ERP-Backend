using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Engines.Workflow.Domain;

public sealed class WorkflowDefinition : AuditableEntity
{

    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public ICollection<WorkflowStep> Steps { get; set; } = new List<WorkflowStep>();

}
