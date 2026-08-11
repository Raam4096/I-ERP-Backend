using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Assets.Domain;

public sealed class AssetType : AuditableEntity
{

    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

}
