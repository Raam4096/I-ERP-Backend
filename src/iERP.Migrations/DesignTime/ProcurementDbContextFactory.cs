using iERP.Modules.Procurement.Infrastructure;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Migrations.DesignTime;

public sealed class ProcurementDbContextFactory : DesignTimeDbContextFactoryBase<ProcurementDbContext>
{
    protected override ProcurementDbContext Create(DbContextOptions<ProcurementDbContext> options, ITenantContext tenantContext)
        => new(options, tenantContext);
}
