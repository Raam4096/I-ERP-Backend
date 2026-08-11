using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Reporting.Domain;

public sealed class ReportDefinition : AuditableEntity
{

    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? QueryKey { get; set; }
    public bool IsActive { get; set; } = true;

}
