using iERP.Infrastructure.Persistence;
using iERP.Modules.AI.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.AI.Infrastructure.Configurations;

public sealed class AIToolDefinitionConfiguration : AuditableEntityConfiguration<AIToolDefinition>
{
    public override void Configure(EntityTypeBuilder<AIToolDefinition> builder)
    {
        base.Configure(builder);
        builder.ToTable("ai_tool_definitions", "ai");
        builder.HasIndex(x => new { x.TenantId, x.Name }).IsUnique();
        builder.Property(x => x.Name).HasMaxLength(128);
    }
}
