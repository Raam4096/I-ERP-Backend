using iERP.Infrastructure.Persistence;
using iERP.Modules.CRM.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.CRM.Infrastructure.Configurations;

public sealed class LeadFollowUpConfiguration : AuditableEntityConfiguration<LeadFollowUp>
{
    public override void Configure(EntityTypeBuilder<LeadFollowUp> builder)
    {
        base.Configure(builder);
        builder.ToTable("lead_followups", "crm");

        builder.Property(x => x.ActivityType).HasMaxLength(128).IsRequired();
        builder.Property(x => x.Remarks).HasMaxLength(4000);
        builder.Property(x => x.Status).HasMaxLength(64).IsRequired();
        builder.Property(x => x.FollowUpDate).IsRequired();

        builder.HasIndex(x => new { x.TenantId, x.LeadId, x.FollowUpDate });
        builder.HasIndex(x => new { x.TenantId, x.Status });

        builder.HasMany(x => x.Attachments)
            .WithOne(x => x.FollowUp)
            .HasForeignKey(x => x.FollowUpId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(x => x.Attachments)
            .HasField("_attachments")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
