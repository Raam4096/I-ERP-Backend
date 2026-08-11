using iERP.Modules.Catalog.Infrastructure;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Migrations.DesignTime;

public sealed class CatalogDbContextFactory : DesignTimeDbContextFactoryBase<CatalogDbContext>
{
    protected override CatalogDbContext Create(DbContextOptions<CatalogDbContext> options, ITenantContext tenantContext)
        => new(options, tenantContext);
}
