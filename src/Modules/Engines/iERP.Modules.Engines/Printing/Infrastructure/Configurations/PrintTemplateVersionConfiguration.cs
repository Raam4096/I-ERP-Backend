using iERP.Infrastructure.Persistence;
using iERP.Modules.Engines.Printing.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Engines.Printing.Infrastructure.Configurations;

public sealed class PrintTemplateVersionConfiguration : AuditableEntityConfiguration<PrintTemplateVersion>
{
    public override void Configure(EntityTypeBuilder<PrintTemplateVersion> builder)
    {
        base.Configure(builder);
        builder.ToTable("print_template_versions", "printing");
        builder.HasIndex(x => new { x.TenantId, x.PrintTemplateId, x.TemplateVersionNumber }).IsUnique();
    }
}
