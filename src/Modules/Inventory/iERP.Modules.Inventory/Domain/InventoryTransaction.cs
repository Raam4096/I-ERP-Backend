using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Inventory.Domain;

public sealed class InventoryTransaction : AuditableEntity
{

    public Guid SubsidiaryId { get; set; }
    public string DocumentNo { get; set; } = string.Empty;
    public DateOnly TransactionDate { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public string Status { get; set; } = "draft";
    public string? ReferenceEntityName { get; set; }
    public Guid? ReferenceRecordId { get; set; }

}
