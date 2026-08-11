using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.Notifications.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Platform.Notifications.Infrastructure.Configurations;

public sealed class NotificationLogConfiguration : AuditableEntityConfiguration<NotificationLog>
{
    public override void Configure(EntityTypeBuilder<NotificationLog> builder)
    {
        base.Configure(builder);
        builder.ToTable("notification_logs", "notifications");
        builder.HasIndex(x => new { x.TenantId, x.UserId, x.CreatedAt });
    }
}
