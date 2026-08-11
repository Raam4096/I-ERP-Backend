using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.Settings.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Platform.Settings.Infrastructure.Configurations;

public sealed class SystemSettingConfiguration : AuditableEntityConfiguration<SystemSetting>
{
    public override void Configure(EntityTypeBuilder<SystemSetting> builder)
    {
        base.Configure(builder);
        builder.ToTable("system_settings", "organization");
        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.Key }).IsUnique();
    }
}
