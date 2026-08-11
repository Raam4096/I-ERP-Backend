using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Finance.Domain;

public sealed class JournalEntryLine : AuditableEntity
{

    public Guid JournalEntryId { get; set; }
    public int LineNo { get; set; }
    public Guid AccountId { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public decimal ExchangeRate { get; set; } = 1m;
    public decimal BaseDebit { get; set; }
    public decimal BaseCredit { get; set; }
    public Guid? CostCenterId { get; set; }
    public Guid? ClassId { get; set; }

}
