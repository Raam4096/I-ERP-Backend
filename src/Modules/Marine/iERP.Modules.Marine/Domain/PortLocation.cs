using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Marine.Domain;

public sealed class PortLocation : AuditableEntity
{

    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Country { get; set; }
    public bool IsActive { get; set; } = true;

}
