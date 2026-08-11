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
        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.OpportunityNumber }).IsUnique();
    }
}
