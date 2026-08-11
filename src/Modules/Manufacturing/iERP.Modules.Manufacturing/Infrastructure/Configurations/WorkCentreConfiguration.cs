using iERP.Infrastructure.Persistence;
using iERP.Modules.Manufacturing.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Manufacturing.Infrastructure.Configurations;

public sealed class WorkCentreConfiguration : AuditableEntityConfiguration<WorkCentre>
{
    public override void Configure(EntityTypeBuilder<WorkCentre> builder)
    {
        base.Configure(builder);
        builder.ToTable("work_centres", "manufacturing");
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
    }
}
