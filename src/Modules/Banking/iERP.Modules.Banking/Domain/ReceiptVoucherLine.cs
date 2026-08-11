using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Banking.Domain;

public sealed class ReceiptVoucherLine : AuditableEntity
{

    public Guid ReceiptVoucherId { get; set; }
    public int LineNo { get; set; }
    public Guid? AccountId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }

}
