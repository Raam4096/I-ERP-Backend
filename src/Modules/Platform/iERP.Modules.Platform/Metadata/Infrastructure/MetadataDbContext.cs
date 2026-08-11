using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.Metadata.Domain;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.Platform.Metadata.Infrastructure;

public sealed class MetadataDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public MetadataDbContext(DbContextOptions<MetadataDbContext> options, ITenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<ModuleDefinition> ModuleDefinitions => Set<ModuleDefinition>();
    public DbSet<ScreenDefinition> ScreenDefinitions => Set<ScreenDefinition>();
    public DbSet<SectionDefinition> SectionDefinitions => Set<SectionDefinition>();
    public DbSet<FieldDefinition> FieldDefinitions => Set<FieldDefinition>();
    public DbSet<CustomFieldDefinition> CustomFieldDefinitions => Set<CustomFieldDefinition>();
    public DbSet<CustomFieldValue> CustomFieldValues => Set<CustomFieldValue>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("metadata");
        modelBuilder.ApplyConfigurationsFromNamespace(
            typeof(MetadataDbContext).Assembly,
            "iERP.Modules.Platform.Metadata.Infrastructure.Configurations");
        modelBuilder.ApplySnakeCaseNamingConvention();
        modelBuilder.ConfigureMoneyPrecision();
        modelBuilder.ApplyTenantAndSoftDeleteFilters(_tenantContext);
        base.OnModelCreating(modelBuilder);
    }
}
