using iERP.Infrastructure.Persistence;
using iERP.Modules.Manufacturing.Domain;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.Manufacturing.Infrastructure;

public sealed class ManufacturingDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public ManufacturingDbContext(DbContextOptions<ManufacturingDbContext> options, ITenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<BillOfMaterials> BillOfMaterials => Set<BillOfMaterials>();
    public DbSet<BillOfMaterialsLine> BillOfMaterialsLines => Set<BillOfMaterialsLine>();
    public DbSet<WorkCentre> WorkCentres => Set<WorkCentre>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    public DbSet<WorkOrderLine> WorkOrderLines => Set<WorkOrderLine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("manufacturing");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ManufacturingDbContext).Assembly);
        modelBuilder.ApplySnakeCaseNamingConvention();
        modelBuilder.ConfigureMoneyPrecision();
        modelBuilder.ApplyTenantAndSoftDeleteFilters(_tenantContext);
        base.OnModelCreating(modelBuilder);
    }
}
