using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Inventory.Domain;

public sealed class StockTransfer : AuditableEntity
{

    public Guid SubsidiaryId { get; set; }
    public string DocumentNo { get; set; } = string.Empty;
    public DateOnly TransferDate { get; set; }
    public Guid FromWarehouseId { get; set; }
    public Guid ToWarehouseId { get; set; }
    public string Status { get; set; } = "draft";

}
