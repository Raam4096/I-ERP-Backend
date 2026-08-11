using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Procurement.Domain;

public sealed class GoodsReceivedNoteLine : AuditableEntity
{

    public Guid GoodsReceivedNoteId { get; set; }
    public int LineNo { get; set; }
    public Guid ItemId { get; set; }
    public string? Description { get; set; }
    public decimal Quantity { get; set; }
    public Guid UomId { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
    public Guid? TaxCodeId { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineAmount { get; set; }

}
