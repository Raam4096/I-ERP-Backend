using iERP.Modules.Finance.Infrastructure;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Migrations.DesignTime;

public sealed class FinanceDbContextFactory : DesignTimeDbContextFactoryBase<FinanceDbContext>
{
    protected override FinanceDbContext Create(DbContextOptions<FinanceDbContext> options, ITenantContext tenantContext)
        => new(options, tenantContext);
}
