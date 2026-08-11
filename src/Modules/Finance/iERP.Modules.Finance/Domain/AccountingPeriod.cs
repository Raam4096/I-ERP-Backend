using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Finance.Domain;

public sealed class AccountingPeriod : AuditableEntity
{

    public Guid FiscalYearId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public bool IsClosed { get; set; }

}
