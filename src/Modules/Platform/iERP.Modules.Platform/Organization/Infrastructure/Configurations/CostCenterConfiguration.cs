using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.Organization.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Platform.Organization.Infrastructure.Configurations;

public sealed class CostCenterConfiguration : AuditableEntityConfiguration<CostCenter>
{
    public override void Configure(EntityTypeBuilder<CostCenter> builder)
    {
        base.Configure(builder);
        builder.ToTable("cost_centers", "organization");
        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.Code }).IsUnique();
    }
}
