using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Platform.Settings.Domain;

public sealed class DocumentSequence : AuditableEntity
{

    public Guid SubsidiaryId { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string Prefix { get; set; } = string.Empty;
    public long NextNumber { get; set; } = 1;
    public int Padding { get; set; } = 6;

}
