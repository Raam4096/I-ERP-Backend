using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Engines.Bridge.Domain;

public sealed class BridgeDefinition : AuditableEntity
{

    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string SourceEntityName { get; set; } = string.Empty;
    public string TargetEntityName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

}
