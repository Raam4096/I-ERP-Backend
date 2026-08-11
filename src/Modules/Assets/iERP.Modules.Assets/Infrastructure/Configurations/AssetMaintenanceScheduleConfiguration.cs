using iERP.Infrastructure.Persistence;
using iERP.Modules.Assets.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Assets.Infrastructure.Configurations;

public sealed class AssetMaintenanceScheduleConfiguration : AuditableEntityConfiguration<AssetMaintenanceSchedule>
{
    public override void Configure(EntityTypeBuilder<AssetMaintenanceSchedule> builder)
    {
        base.Configure(builder);
        builder.ToTable("asset_maintenance_schedules", "assets");

    }
}
