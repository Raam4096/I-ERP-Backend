using iERP.Infrastructure.Persistence;
using iERP.Modules.CRM.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.CRM.Infrastructure.Configurations;

public sealed class CustomerConfiguration : AuditableEntityConfiguration<Customer>
{
    public override void Configure(EntityTypeBuilder<Customer> builder)
    {
        base.Configure(builder);
        builder.ToTable("customers", "crm");
        builder.HasIndex(x => new { x.TenantId, x.CustomerCode }).IsUnique();
    }
}
