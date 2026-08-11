using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Finance.Domain;

public sealed class FiscalYear : AuditableEntity
{

    public Guid SubsidiaryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public bool IsClosed { get; set; }

}
