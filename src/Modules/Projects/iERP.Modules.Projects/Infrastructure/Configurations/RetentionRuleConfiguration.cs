using iERP.Infrastructure.Persistence;
using iERP.Modules.Projects.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Projects.Infrastructure.Configurations;

public sealed class RetentionRuleConfiguration : AuditableEntityConfiguration<RetentionRule>
{
    public override void Configure(EntityTypeBuilder<RetentionRule> builder)
    {
        base.Configure(builder);
        builder.ToTable("retention_rules", "projects");

    }
}
