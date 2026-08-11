using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Finance.Domain;

public sealed class Budget : AuditableEntity
{

    public Guid SubsidiaryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid FiscalYearId { get; set; }
    public string Status { get; set; } = "draft";

}
