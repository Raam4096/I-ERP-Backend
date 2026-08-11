using iERP.Modules.CRM.Infrastructure;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Migrations.DesignTime;

public sealed class CrmDbContextFactory : DesignTimeDbContextFactoryBase<CrmDbContext>
{
    protected override CrmDbContext Create(DbContextOptions<CrmDbContext> options, ITenantContext tenantContext)
        => new(options, tenantContext);
}
