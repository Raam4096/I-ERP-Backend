using iERP.Infrastructure.Persistence;
using iERP.Modules.CRM.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.CRM.Infrastructure.Configurations;

public sealed class AddressConfiguration : AuditableEntityConfiguration<Address>
{
    public override void Configure(EntityTypeBuilder<Address> builder)
    {
        base.Configure(builder);
        builder.ToTable("addresses", "crm");
        builder.HasIndex(x => new { x.TenantId, x.CustomerId, x.AddressType });
    }
}
