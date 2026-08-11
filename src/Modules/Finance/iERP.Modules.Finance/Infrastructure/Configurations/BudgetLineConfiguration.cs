using iERP.Infrastructure.Persistence;
using iERP.Modules.Finance.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Finance.Infrastructure.Configurations;

public sealed class BudgetLineConfiguration : AuditableEntityConfiguration<BudgetLine>
{
    public override void Configure(EntityTypeBuilder<BudgetLine> builder)
    {
        base.Configure(builder);
        builder.ToTable("budget_lines", "finance");

    }
}
