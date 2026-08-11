using iERP.Infrastructure.Persistence;
using iERP.Modules.Banking.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Banking.Infrastructure.Configurations;

public sealed class BankAccountConfiguration : AuditableEntityConfiguration<BankAccount>
{
    public override void Configure(EntityTypeBuilder<BankAccount> builder)
    {
        base.Configure(builder);
        builder.ToTable("bank_accounts", "banking");
        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.AccountCode }).IsUnique();
    }
}
