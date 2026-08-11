using iERP.Infrastructure.Persistence;
using iERP.Modules.Finance.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Finance.Infrastructure.Configurations;

public sealed class BudgetConfiguration : AuditableEntityConfiguration<Budget>
{
    public override void Configure(EntityTypeBuilder<Budget> builder)
    {
        base.Configure(builder);
        builder.ToTable("budgets", "finance");

    }
}
