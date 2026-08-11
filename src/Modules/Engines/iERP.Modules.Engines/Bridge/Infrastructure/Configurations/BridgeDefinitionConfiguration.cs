using iERP.Infrastructure.Persistence;
using iERP.Modules.Engines.Bridge.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Engines.Bridge.Infrastructure.Configurations;

public sealed class BridgeDefinitionConfiguration : AuditableEntityConfiguration<BridgeDefinition>
{
    public override void Configure(EntityTypeBuilder<BridgeDefinition> builder)
    {
        base.Configure(builder);
        builder.ToTable("bridge_definitions", "bridge");
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
    }
}
