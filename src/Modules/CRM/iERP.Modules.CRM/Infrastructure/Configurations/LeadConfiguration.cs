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
        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.LeadNumber }).IsUnique();
    }
}
