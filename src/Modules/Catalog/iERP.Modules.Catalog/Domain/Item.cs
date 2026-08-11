using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Catalog.Domain;

public sealed class Item : AuditableEntity
{

    public string ItemCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ItemType { get; set; } = "product";
    public Guid? CategoryId { get; set; }
    public Guid UomId { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal CostPrice { get; set; }
    public decimal? ReorderLevel { get; set; }
    public decimal? ReorderQuantity { get; set; }
    public Guid? SalesTaxCodeId { get; set; }
    public Guid? PurchaseTaxCodeId { get; set; }
    public Guid? SalesGlAccountId { get; set; }
    public Guid? PurchaseGlAccountId { get; set; }
    public Guid? InventoryGlAccountId { get; set; }
    public bool IsActive { get; set; } = true;

}
