using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Banking.Domain;

public sealed class ReceiptVoucher : AuditableEntity
{

    public Guid SubsidiaryId { get; set; }
    public string DocumentNo { get; set; } = string.Empty;
    public DateOnly DocumentDate { get; set; }
    public Guid BankAccountId { get; set; }
    public Guid? CustomerId { get; set; }
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public string Status { get; set; } = "draft";

}
