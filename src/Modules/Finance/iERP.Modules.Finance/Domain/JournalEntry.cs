using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Finance.Domain;

public sealed class JournalEntry : AuditableEntity
{

    public Guid SubsidiaryId { get; set; }
    public string DocumentNo { get; set; } = string.Empty;
    public DateOnly PostingDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "draft";

}
