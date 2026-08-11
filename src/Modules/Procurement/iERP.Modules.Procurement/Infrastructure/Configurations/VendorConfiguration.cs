using iERP.Infrastructure.Persistence;
using iERP.Modules.Procurement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Procurement.Infrastructure.Configurations;

public sealed class VendorConfiguration : AuditableEntityConfiguration<Vendor>
{
    public override void Configure(EntityTypeBuilder<Vendor> builder)
    {
        base.Configure(builder);
        builder.ToTable("vendors", "procurement");
        builder.HasIndex(x => new { x.TenantId, x.VendorCode }).IsUnique();
    }
}
