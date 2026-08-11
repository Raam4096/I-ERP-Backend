using iERP.Infrastructure.Persistence;
using iERP.Modules.Banking.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Banking.Infrastructure.Configurations;

public sealed class BankReconciliationConfiguration : AuditableEntityConfiguration<BankReconciliation>
{
    public override void Configure(EntityTypeBuilder<BankReconciliation> builder)
    {
        base.Configure(builder);
        builder.ToTable("bank_reconciliations", "banking");

    }
}
