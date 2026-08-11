using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Catalog.Domain;

public sealed class ItemCategory : AuditableEntity
{

    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? ParentCategoryId { get; set; }
    public bool IsActive { get; set; } = true;

}
