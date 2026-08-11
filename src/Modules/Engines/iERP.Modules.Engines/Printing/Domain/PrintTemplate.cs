using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Engines.Printing.Domain;

public sealed class PrintTemplate : AuditableEntity
{

    public string EntityName { get; set; } = string.Empty;
    public string TemplateCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

}
