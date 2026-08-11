using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.Organization.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Platform.Organization.Infrastructure.Configurations;

public sealed class ReportingDimensionConfiguration : AuditableEntityConfiguration<ReportingDimension>
{
    public override void Configure(EntityTypeBuilder<ReportingDimension> builder)
    {
        base.Configure(builder);
        builder.ToTable("reporting_dimensions", "organization");
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
    }
}
