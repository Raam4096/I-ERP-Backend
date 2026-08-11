using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Platform.Organization.Domain;

public sealed class Department : AuditableEntity
{

    public Guid SubsidiaryId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

}
