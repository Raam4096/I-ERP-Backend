using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Inventory.Domain;

public sealed class Warehouse : AuditableEntity
{

    public Guid SubsidiaryId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

}
