using iERP.Infrastructure.Persistence;
using iERP.Modules.Engines.Printing.Domain;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.Engines.Printing.Infrastructure;

public sealed class PrintingDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public PrintingDbContext(DbContextOptions<PrintingDbContext> options, ITenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<PrintTemplate> PrintTemplates => Set<PrintTemplate>();
    public DbSet<PrintTemplateVersion> PrintTemplateVersions => Set<PrintTemplateVersion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("printing");
        modelBuilder.ApplyConfigurationsFromNamespace(
            typeof(PrintingDbContext).Assembly,
            "iERP.Modules.Engines.Printing.Infrastructure.Configurations");
        modelBuilder.ApplySnakeCaseNamingConvention();
        modelBuilder.ConfigureMoneyPrecision();
        modelBuilder.ApplyTenantAndSoftDeleteFilters(_tenantContext);
        base.OnModelCreating(modelBuilder);
    }
}
