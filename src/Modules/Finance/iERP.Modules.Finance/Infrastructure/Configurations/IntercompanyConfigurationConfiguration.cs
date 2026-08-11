using iERP.Infrastructure.Persistence;
using iERP.Modules.Finance.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Finance.Infrastructure.Configurations;

public sealed class IntercompanyConfigurationConfiguration : AuditableEntityConfiguration<IntercompanyConfiguration>
{
    public override void Configure(EntityTypeBuilder<IntercompanyConfiguration> builder)
    {
        base.Configure(builder);
        builder.ToTable("intercompany_configurations", "finance");
        builder.HasIndex(x => new { x.TenantId, x.SourceSubsidiaryId, x.TargetSubsidiaryId }).IsUnique();
    }
}
