using iERP.Modules.Platform.Organization.Infrastructure;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Migrations.DesignTime;

public sealed class OrganizationDbContextFactory : DesignTimeDbContextFactoryBase<OrganizationDbContext>
{
    protected override OrganizationDbContext Create(DbContextOptions<OrganizationDbContext> options, ITenantContext tenantContext)
        => new(options, tenantContext);
}
