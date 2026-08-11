using iERP.Modules.Inventory.Infrastructure;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Migrations.DesignTime;

public sealed class InventoryDbContextFactory : DesignTimeDbContextFactoryBase<InventoryDbContext>
{
    protected override InventoryDbContext Create(DbContextOptions<InventoryDbContext> options, ITenantContext tenantContext)
        => new(options, tenantContext);
}
