using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Finance.Domain;

public sealed class IntercompanyConfiguration : AuditableEntity
{

    public Guid SourceSubsidiaryId { get; set; }
    public Guid TargetSubsidiaryId { get; set; }
    public Guid? ReceivableAccountId { get; set; }
    public Guid? PayableAccountId { get; set; }
    public bool IsActive { get; set; } = true;

}
