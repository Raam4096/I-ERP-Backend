using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Engines.Printing.Domain;

public sealed class PrintTemplateVersion : AuditableEntity
{

    public Guid PrintTemplateId { get; set; }
    public int TemplateVersionNumber { get; set; }
    public string TemplateContent { get; set; } = string.Empty;
    public string OutputType { get; set; } = "pdf";
    public bool IsActive { get; set; } = true;

}
