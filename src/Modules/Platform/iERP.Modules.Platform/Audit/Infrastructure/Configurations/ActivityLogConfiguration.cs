using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.Audit.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Platform.Audit.Infrastructure.Configurations;

public sealed class ActivityLogConfiguration : AuditableEntityConfiguration<ActivityLog>
{
    public override void Configure(EntityTypeBuilder<ActivityLog> builder)
    {
        base.Configure(builder);
        builder.ToTable("activity_logs", "audit");
        builder.Property(x => x.OldValue).HasColumnType("jsonb");
        builder.Property(x => x.NewValue).HasColumnType("jsonb");
        builder.HasIndex(x => new { x.TenantId, x.EntityName, x.RecordId });
    }
}
