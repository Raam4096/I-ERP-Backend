using iERP.Infrastructure.Persistence;
using iERP.Modules.CRM.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.CRM.Infrastructure.Configurations;

public sealed class OpportunityConfiguration : AuditableEntityConfiguration<Opportunity>
{
    public override void Configure(EntityTypeBuilder<Opportunity> builder)
    {
        base.Configure(builder);
        builder.ToTable("opportunities", "crm");

        builder.Property(x => x.OpportunityNumber).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(256).IsRequired();
        builder.Property(x => x.Stage).HasMaxLength(64).IsRequired();
        builder.Property(x => x.OpportunityValue).HasPrecision(19, 4).IsRequired();
        builder.Property(x => x.CurrencyCode).HasMaxLength(16);
        builder.Property(x => x.Status).HasMaxLength(64).IsRequired();
        builder.Property(x => x.StatusBeforeDiscard).HasMaxLength(64);
        builder.Property(x => x.Computations).HasMaxLength(4000);
        builder.Property(x => x.Notes).HasMaxLength(4000);
        builder.Property(x => x.ClosedReason).HasMaxLength(1024);

        builder.HasIndex(x => new { x.TenantId, x.OpportunityNumber }).IsUnique();
        builder.HasIndex(x => new { x.TenantId, x.Status });
        builder.HasIndex(x => new { x.TenantId, x.LeadId });
        builder.HasIndex(x => x.LeadId);

        builder.HasMany(x => x.FollowUps)
            .WithOne(x => x.Opportunity)
            .HasForeignKey(x => x.OpportunityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(x => x.FollowUps)
            .HasField("_followUps")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
