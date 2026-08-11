using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Inventory.Domain;

public sealed class StockReservation : AuditableEntity
{

    public Guid SubsidiaryId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid? BinLocationId { get; set; }
    public Guid ItemId { get; set; }
    public decimal Quantity { get; set; }
    public string SourceEntityName { get; set; } = string.Empty;
    public Guid SourceRecordId { get; set; }
    public string Status { get; set; } = "active";

}
