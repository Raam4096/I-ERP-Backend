using iERP.Infrastructure.Persistence;
using iERP.Modules.CRM.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.CRM.Infrastructure.Configurations;

public sealed class LeadConfiguration : AuditableEntityConfiguration<Lead>
{
    public override void Configure(EntityTypeBuilder<Lead> builder)
    {
        base.Configure(builder);
        builder.ToTable("leads", "crm");

        builder.Property(x => x.LeadNumber).HasMaxLength(64).IsRequired();
        builder.Property(x => x.CompanyName).HasMaxLength(256).IsRequired();
        builder.Property(x => x.ContactPerson).HasMaxLength(256);
        builder.Property(x => x.Phone).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(256).IsRequired();
        builder.Property(x => x.Industry).HasMaxLength(128);
        builder.Property(x => x.Address).HasMaxLength(1024);
        builder.Property(x => x.AnnualRevenue).HasPrecision(19, 4);
        builder.Property(x => x.CompanySize).HasMaxLength(64);
        builder.Property(x => x.LeadSource).HasMaxLength(128);
        builder.Property(x => x.ProjectDescription).HasMaxLength(4000);
        builder.Property(x => x.ProjectType).HasMaxLength(128);
        builder.Property(x => x.Status).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Subsidiary).HasMaxLength(256);
        builder.Property(x => x.Website).HasMaxLength(512);
        builder.Property(x => x.Notes).HasMaxLength(4000);

        builder.HasIndex(x => new { x.TenantId, x.LeadNumber }).IsUnique();
        builder.HasIndex(x => new { x.TenantId, x.Email });
        builder.HasIndex(x => new { x.TenantId, x.Phone });
        builder.HasIndex(x => new { x.TenantId, x.Status });
        builder.HasIndex(x => new { x.TenantId, x.AssignedToUserId });

        // Soft uniqueness for active leads: enforced in application for email/phone duplicates.
        builder.HasMany(x => x.FollowUps)
            .WithOne(x => x.Lead)
            .HasForeignKey(x => x.LeadId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(x => x.FollowUps)
            .HasField("_followUps")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
