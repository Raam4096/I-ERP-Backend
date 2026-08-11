using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Manufacturing.Domain;

public sealed class BillOfMaterials : AuditableEntity
{

    public Guid ItemId { get; set; }
    public string BomVersion { get; set; } = "1.0";
    public bool IsActive { get; set; } = true;

}
