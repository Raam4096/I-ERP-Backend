using iERP.Infrastructure.Persistence;
using iERP.Modules.CRM.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.CRM.Infrastructure.Configurations;

public sealed class OpportunityFollowUpConfiguration : AuditableEntityConfiguration<OpportunityFollowUp>
{
    public override void Configure(EntityTypeBuilder<OpportunityFollowUp> builder)
    {
        base.Configure(builder);
        builder.ToTable("opportunity_followups", "crm");

        builder.Property(x => x.ActivityType).HasMaxLength(128).IsRequired();
        builder.Property(x => x.FollowUpDate).IsRequired();
        builder.Property(x => x.Remarks).HasMaxLength(4000);
        builder.Property(x => x.Status).HasMaxLength(64).IsRequired();

        builder.HasIndex(x => new { x.TenantId, x.OpportunityId, x.FollowUpDate });
        builder.HasIndex(x => x.OpportunityId);
    }
}
