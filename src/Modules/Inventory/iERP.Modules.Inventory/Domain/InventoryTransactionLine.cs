using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Inventory.Domain;

public sealed class InventoryTransactionLine : AuditableEntity
{

    public Guid InventoryTransactionId { get; set; }
    public int LineNo { get; set; }
    public Guid ItemId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid? BinLocationId { get; set; }
    public decimal Quantity { get; set; }
    public Guid UomId { get; set; }
    public decimal? UnitCost { get; set; }

}
