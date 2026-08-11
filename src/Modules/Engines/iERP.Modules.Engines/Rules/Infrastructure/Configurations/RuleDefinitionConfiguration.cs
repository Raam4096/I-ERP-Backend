using iERP.Infrastructure.Persistence;
using iERP.Modules.Engines.Rules.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Engines.Rules.Infrastructure.Configurations;

public sealed class RuleDefinitionConfiguration : AuditableEntityConfiguration<RuleDefinition>
{
    public override void Configure(EntityTypeBuilder<RuleDefinition> builder)
    {
        base.Configure(builder);
        builder.ToTable("rule_definitions", "rules");
        builder.Property(x => x.Conditions).HasColumnType("jsonb");
        builder.Property(x => x.Actions).HasColumnType("jsonb");
        builder.HasIndex(x => new { x.TenantId, x.EntityName, x.EventName, x.Priority });
    }
}
