using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Catalog.Domain;

public sealed class PriceListItem : AuditableEntity
{

    public Guid PriceListId { get; set; }
    public Guid ItemId { get; set; }
    public decimal UnitPrice { get; set; }
    public DateOnly? EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }

}
