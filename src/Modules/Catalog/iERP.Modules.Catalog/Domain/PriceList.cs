using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Catalog.Domain;

public sealed class PriceList : AuditableEntity
{

    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = "USD";
    public bool IsActive { get; set; } = true;

}
