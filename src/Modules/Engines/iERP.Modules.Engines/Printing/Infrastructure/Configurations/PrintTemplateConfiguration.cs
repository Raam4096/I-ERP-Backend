using iERP.Infrastructure.Persistence;
using iERP.Modules.Engines.Printing.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Engines.Printing.Infrastructure.Configurations;

public sealed class PrintTemplateConfiguration : AuditableEntityConfiguration<PrintTemplate>
{
    public override void Configure(EntityTypeBuilder<PrintTemplate> builder)
    {
        base.Configure(builder);
        builder.ToTable("print_templates", "printing");
        builder.HasIndex(x => new { x.TenantId, x.EntityName, x.TemplateCode }).IsUnique();
    }
}
