using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Engines.Bridge.Domain;

public sealed class BridgeLog : AuditableEntity
{

    public Guid BridgeDefinitionId { get; set; }
    public Guid SourceRecordId { get; set; }
    public Guid? TargetRecordId { get; set; }
    public string Status { get; set; } = "pending";
    public string? ErrorMessage { get; set; }

}
