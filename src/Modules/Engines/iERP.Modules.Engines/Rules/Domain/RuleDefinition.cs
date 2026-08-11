using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Engines.Rules.Domain;

public sealed class RuleDefinition : AuditableEntity
{

    public string EntityName { get; set; } = string.Empty;
    public string EventName { get; set; } = string.Empty;
    public string Conditions { get; set; } = "[]";
    public string Actions { get; set; } = "[]";
    public int Priority { get; set; }
    public bool IsActive { get; set; } = true;

}
