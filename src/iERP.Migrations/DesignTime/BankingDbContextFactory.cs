using iERP.Modules.Banking.Infrastructure;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Migrations.DesignTime;

public sealed class BankingDbContextFactory : DesignTimeDbContextFactoryBase<BankingDbContext>
{
    protected override BankingDbContext Create(DbContextOptions<BankingDbContext> options, ITenantContext tenantContext)
        => new(options, tenantContext);
}
