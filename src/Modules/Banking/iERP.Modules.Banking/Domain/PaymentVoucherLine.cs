using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Banking.Domain;

public sealed class PaymentVoucherLine : AuditableEntity
{

    public Guid PaymentVoucherId { get; set; }
    public int LineNo { get; set; }
    public Guid? AccountId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }

}
