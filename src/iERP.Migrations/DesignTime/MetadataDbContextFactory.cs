using iERP.Modules.Platform.Metadata.Infrastructure;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Migrations.DesignTime;

public sealed class MetadataDbContextFactory : DesignTimeDbContextFactoryBase<MetadataDbContext>
{
    protected override MetadataDbContext Create(DbContextOptions<MetadataDbContext> options, ITenantContext tenantContext)
        => new(options, tenantContext);
}
