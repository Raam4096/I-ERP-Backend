using iERP.Infrastructure.Persistence;
using iERP.Modules.Engines.Bridge.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Engines.Bridge.Infrastructure.Configurations;

public sealed class BridgeMappingConfiguration : AuditableEntityConfiguration<BridgeMapping>
{
    public override void Configure(EntityTypeBuilder<BridgeMapping> builder)
    {
        base.Configure(builder);
        builder.ToTable("bridge_mappings", "bridge");
        builder.HasIndex(x => new { x.TenantId, x.BridgeDefinitionId, x.SourceField });
    }
}
