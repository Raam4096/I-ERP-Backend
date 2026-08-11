using iERP.Infrastructure.Persistence;
using iERP.Modules.AI.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.AI.Infrastructure.Configurations;

public sealed class AIToolPermissionConfiguration : AuditableEntityConfiguration<AIToolPermission>
{
    public override void Configure(EntityTypeBuilder<AIToolPermission> builder)
    {
        base.Configure(builder);
        builder.ToTable("ai_tool_permissions", "ai");
        builder.HasIndex(x => new { x.TenantId, x.AIToolDefinitionId, x.RoleId, x.AllowedExecutionMode }).IsUnique();
    }
}
