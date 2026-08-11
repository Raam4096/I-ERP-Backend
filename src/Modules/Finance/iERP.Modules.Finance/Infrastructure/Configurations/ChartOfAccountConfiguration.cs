using iERP.Infrastructure.Persistence;
using iERP.Modules.Finance.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Finance.Infrastructure.Configurations;

public sealed class ChartOfAccountConfiguration : AuditableEntityConfiguration<ChartOfAccount>
{
    public override void Configure(EntityTypeBuilder<ChartOfAccount> builder)
    {
        base.Configure(builder);
        builder.ToTable("chart_of_accounts", "finance");
        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.AccountCode }).IsUnique();
    }
}
