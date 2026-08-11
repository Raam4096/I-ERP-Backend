using iERP.Infrastructure.Persistence;
using iERP.Modules.Reporting.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Reporting.Infrastructure.Configurations;

public sealed class ReportDefinitionConfiguration : AuditableEntityConfiguration<ReportDefinition>
{
    public override void Configure(EntityTypeBuilder<ReportDefinition> builder)
    {
        base.Configure(builder);
        builder.ToTable("report_definitions", "reporting");
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
    }
}
