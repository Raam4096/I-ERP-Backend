using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Inventory.Domain;

public sealed class StockTransferLine : AuditableEntity
{

    public Guid StockTransferId { get; set; }
    public int LineNo { get; set; }
    public Guid ItemId { get; set; }
    public decimal Quantity { get; set; }
    public Guid UomId { get; set; }

}
