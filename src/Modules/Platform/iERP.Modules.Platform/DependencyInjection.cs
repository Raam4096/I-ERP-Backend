using iERP.Application.Abstractions.Seeding;
using iERP.Infrastructure.Persistence.Interceptors;
using iERP.Modules.Platform.Identity.Application.Seeding;
using iERP.Modules.Platform.Identity.Infrastructure;
using iERP.Modules.Platform.Metadata.Application.Seeding;
using iERP.Modules.Platform.Metadata.Infrastructure;
using iERP.Modules.Platform.Organization.Infrastructure;
using iERP.Modules.Platform.Tenancy.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace iERP.Modules.Platform;

public static class DependencyInjection
{
    public static IServiceCollection AddPlatformModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PrimaryDatabase")
            ?? "Host=localhost;Port=5432;Database=ierp;Username=ierp;Password=ierp";

        services.AddDbContext<PlatformDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, b => b.MigrationsAssembly("iERP.Migrations"));
            options.AddInterceptors(
                sp.GetRequiredService<TenantSaveChangesInterceptor>(),
                sp.GetRequiredService<AuditSaveChangesInterceptor>());
        });

        services.AddDbContext<IdentityDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, b => b.MigrationsAssembly("iERP.Migrations"));
            options.AddInterceptors(
                sp.GetRequiredService<TenantSaveChangesInterceptor>(),
                sp.GetRequiredService<AuditSaveChangesInterceptor>());
        });

        services.AddDbContext<OrganizationDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, b => b.MigrationsAssembly("iERP.Migrations"));
            options.AddInterceptors(
                sp.GetRequiredService<TenantSaveChangesInterceptor>(),
                sp.GetRequiredService<AuditSaveChangesInterceptor>());
        });

        services.AddDbContext<MetadataDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, b => b.MigrationsAssembly("iERP.Migrations"));
            options.AddInterceptors(
                sp.GetRequiredService<TenantSaveChangesInterceptor>(),
                sp.GetRequiredService<AuditSaveChangesInterceptor>());
        });

        services.AddScoped<IDataSeeder, SystemRoleSeeder>();
        services.AddScoped<IDataSeeder, SystemPermissionSeeder>();
        services.AddScoped<IDataSeeder, DefaultMetadataSeeder>();

        return services;
    }
}
