using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.Attachments.Domain;
using iERP.Modules.Platform.Audit.Domain;
using iERP.Modules.Platform.DynamicModules.Domain;
using iERP.Modules.Platform.Notifications.Domain;
using iERP.Modules.Platform.Tenancy.Domain;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.Platform.Tenancy.Infrastructure;

public sealed class PlatformDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public PlatformDbContext(DbContextOptions<PlatformDbContext> options, ITenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<NotificationLog> NotificationLogs => Set<NotificationLog>();
    public DbSet<DynamicModuleDefinition> DynamicModuleDefinitions => Set<DynamicModuleDefinition>();
    public DbSet<DynamicEntityDefinition> DynamicEntityDefinitions => Set<DynamicEntityDefinition>();
    public DbSet<DynamicFieldDefinition> DynamicFieldDefinitions => Set<DynamicFieldDefinition>();
    public DbSet<DynamicRecord> DynamicRecords => Set<DynamicRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var assembly = typeof(PlatformDbContext).Assembly;
        modelBuilder.HasDefaultSchema("platform");
        modelBuilder.ApplyConfigurationsFromNamespace(assembly, "iERP.Modules.Platform.Tenancy.Infrastructure.Configurations");
        modelBuilder.ApplyConfigurationsFromNamespace(assembly, "iERP.Modules.Platform.Audit.Infrastructure.Configurations");
        modelBuilder.ApplyConfigurationsFromNamespace(assembly, "iERP.Modules.Platform.Attachments.Infrastructure.Configurations");
        modelBuilder.ApplyConfigurationsFromNamespace(assembly, "iERP.Modules.Platform.Notifications.Infrastructure.Configurations");
        modelBuilder.ApplyConfigurationsFromNamespace(assembly, "iERP.Modules.Platform.DynamicModules.Infrastructure.Configurations");
        modelBuilder.ApplySnakeCaseNamingConvention();
        modelBuilder.ConfigureMoneyPrecision();
        modelBuilder.ApplyTenantAndSoftDeleteFilters(_tenantContext);
        base.OnModelCreating(modelBuilder);
    }
}
