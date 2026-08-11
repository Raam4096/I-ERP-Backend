using iERP.Infrastructure.Persistence;
using iERP.Modules.Finance.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Finance.Infrastructure.Configurations;

public sealed class CurrencyConfiguration : AuditableEntityConfiguration<Currency>
{
    public override void Configure(EntityTypeBuilder<Currency> builder)
    {
        base.Configure(builder);
        builder.ToTable("currencies", "finance");
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
    }
}
