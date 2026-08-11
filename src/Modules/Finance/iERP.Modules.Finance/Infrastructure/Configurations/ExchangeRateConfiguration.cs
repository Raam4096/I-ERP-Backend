using iERP.Infrastructure.Persistence;
using iERP.Modules.Finance.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Finance.Infrastructure.Configurations;

public sealed class ExchangeRateConfiguration : AuditableEntityConfiguration<ExchangeRate>
{
    public override void Configure(EntityTypeBuilder<ExchangeRate> builder)
    {
        base.Configure(builder);
        builder.ToTable("exchange_rates", "finance");
        builder.HasIndex(x => new { x.TenantId, x.FromCurrencyCode, x.ToCurrencyCode, x.RateDate }).IsUnique();
    }
}
