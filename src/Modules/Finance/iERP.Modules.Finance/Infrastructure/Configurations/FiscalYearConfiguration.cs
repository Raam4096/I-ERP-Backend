using iERP.Infrastructure.Persistence;
using iERP.Modules.Finance.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Finance.Infrastructure.Configurations;

public sealed class FiscalYearConfiguration : AuditableEntityConfiguration<FiscalYear>
{
    public override void Configure(EntityTypeBuilder<FiscalYear> builder)
    {
        base.Configure(builder);
        builder.ToTable("fiscal_years", "finance");
        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.Name }).IsUnique();
    }
}
