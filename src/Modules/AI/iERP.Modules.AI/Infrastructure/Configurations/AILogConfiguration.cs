using iERP.Infrastructure.Persistence;
using iERP.Modules.AI.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.AI.Infrastructure.Configurations;

public sealed class AILogConfiguration : AuditableEntityConfiguration<AILog>
{
    public override void Configure(EntityTypeBuilder<AILog> builder)
    {
        base.Configure(builder);
        builder.ToTable("ai_logs", "ai");
        builder.Property(x => x.RollbackPayload).HasColumnType("jsonb");
        builder.HasIndex(x => new { x.TenantId, x.UserId, x.CreatedAt });
    }
}
