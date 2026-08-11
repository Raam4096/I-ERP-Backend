using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Banking.Domain;

public sealed class BankReconciliation : AuditableEntity
{

    public Guid BankAccountId { get; set; }
    public DateOnly StatementDate { get; set; }
    public decimal StatementBalance { get; set; }
    public string Status { get; set; } = "open";

}
