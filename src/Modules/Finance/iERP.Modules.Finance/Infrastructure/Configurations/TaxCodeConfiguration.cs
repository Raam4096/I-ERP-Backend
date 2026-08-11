using iERP.Infrastructure.Persistence;
using iERP.Modules.Finance.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Finance.Infrastructure.Configurations;

public sealed class TaxCodeConfiguration : AuditableEntityConfiguration<TaxCode>
{
    public override void Configure(EntityTypeBuilder<TaxCode> builder)
    {
        base.Configure(builder);
        builder.ToTable("tax_codes", "finance");
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
    }
}
