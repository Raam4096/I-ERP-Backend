using iERP.Infrastructure.Persistence;
using iERP.Modules.Finance.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Finance.Infrastructure.Configurations;

public sealed class WithholdingTaxCodeConfiguration : AuditableEntityConfiguration<WithholdingTaxCode>
{
    public override void Configure(EntityTypeBuilder<WithholdingTaxCode> builder)
    {
        base.Configure(builder);
        builder.ToTable("withholding_tax_codes", "finance");
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
    }
}
