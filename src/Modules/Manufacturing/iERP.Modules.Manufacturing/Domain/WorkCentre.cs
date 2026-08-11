using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Manufacturing.Domain;

public sealed class WorkCentre : AuditableEntity
{

    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

}
