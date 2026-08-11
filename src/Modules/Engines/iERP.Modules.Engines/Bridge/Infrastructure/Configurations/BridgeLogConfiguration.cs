using iERP.Infrastructure.Persistence;
using iERP.Modules.Engines.Bridge.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Engines.Bridge.Infrastructure.Configurations;

public sealed class BridgeLogConfiguration : AuditableEntityConfiguration<BridgeLog>
{
    public override void Configure(EntityTypeBuilder<BridgeLog> builder)
    {
        base.Configure(builder);
        builder.ToTable("bridge_logs", "bridge");
        builder.HasIndex(x => new { x.TenantId, x.BridgeDefinitionId, x.SourceRecordId });
    }
}
