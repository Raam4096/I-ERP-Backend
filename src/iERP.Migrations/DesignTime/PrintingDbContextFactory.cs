using iERP.Modules.Engines.Printing.Infrastructure;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Migrations.DesignTime;

public sealed class PrintingDbContextFactory : DesignTimeDbContextFactoryBase<PrintingDbContext>
{
    protected override PrintingDbContext Create(DbContextOptions<PrintingDbContext> options, ITenantContext tenantContext)
        => new(options, tenantContext);
}
