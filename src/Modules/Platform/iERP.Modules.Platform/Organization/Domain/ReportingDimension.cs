using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Platform.Organization.Domain;

public sealed class ReportingDimension : AuditableEntity
{

    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DimensionType { get; set; } = "class";
    public bool IsActive { get; set; } = true;

}
