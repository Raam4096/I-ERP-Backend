using iERP.Infrastructure.Persistence;
using iERP.Modules.Finance.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Finance.Infrastructure.Configurations;

public sealed class AccountingPeriodConfiguration : AuditableEntityConfiguration<AccountingPeriod>
{
    public override void Configure(EntityTypeBuilder<AccountingPeriod> builder)
    {
        base.Configure(builder);
        builder.ToTable("accounting_periods", "finance");

    }
}
