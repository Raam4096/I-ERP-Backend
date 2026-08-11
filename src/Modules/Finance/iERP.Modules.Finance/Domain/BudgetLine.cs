using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Finance.Domain;

public sealed class BudgetLine : AuditableEntity
{

    public Guid BudgetId { get; set; }
    public Guid AccountId { get; set; }
    public Guid? CostCenterId { get; set; }
    public decimal Amount { get; set; }

}
