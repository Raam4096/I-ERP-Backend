using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.Metadata.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Platform.Metadata.Infrastructure.Configurations;

public sealed class UserFieldPreferenceConfiguration : AuditableEntityConfiguration<UserFieldPreference>
{
    public override void Configure(EntityTypeBuilder<UserFieldPreference> builder)
    {
        base.Configure(builder);
        builder.ToTable("user_field_preferences", "metadata");
        builder.Property(x => x.ScreenCode).HasMaxLength(128).IsRequired();
        builder.Property(x => x.FieldKey).HasMaxLength(128).IsRequired();
        builder.HasIndex(x => new { x.TenantId, x.UserId, x.ScreenCode, x.FieldKey }).IsUnique();
        builder.HasIndex(x => new { x.TenantId, x.UserId, x.ScreenCode });
    }
}
