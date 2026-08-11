using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Projects.Domain;

public sealed class Subcontractor : AuditableEntity
{

    public Guid ProjectId { get; set; }
    public Guid? VendorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Scope { get; set; }
    public decimal? ContractValue { get; set; }
    public bool IsActive { get; set; } = true;

}
