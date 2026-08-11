using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Engines.Bridge.Domain;

public sealed class BridgeMapping : AuditableEntity
{

    public Guid BridgeDefinitionId { get; set; }
    public string SourceField { get; set; } = string.Empty;
    public string TargetField { get; set; } = string.Empty;
    public string? TransformExpression { get; set; }

}
