using FluentValidation;
using iERP.Application.Abstractions.Options;
using iERP.Application.Abstractions.Seeding;
using iERP.Infrastructure.Persistence.Interceptors;
using iERP.Modules.Platform.Identity.Application.Auth;
using iERP.Modules.Platform.Identity.Application.Seeding;
using iERP.Modules.Platform.Identity.Domain;
using iERP.Modules.Platform.Identity.Infrastructure;
using iERP.Modules.Platform.DynamicModules.Application;
using iERP.Modules.Platform.Metadata.Application;
using iERP.Modules.Platform.Metadata.Application.Seeding;
using iERP.Modules.Platform.Metadata.Infrastructure;
using iERP.Modules.Platform.Organization.Infrastructure;
using iERP.Modules.Platform.Tenancy.Infrastructure;
using Microsoft.AspNetCore.Identity;
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

        services.Configure<AuthSeedOptions>(configuration.GetSection(AuthSeedOptions.SectionName));

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

        services.AddScoped<IPasswordHasher<AppUser>, PasswordHasher<AppUser>>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IMetadataScreenService, MetadataScreenService>();
        services.AddScoped<IDynamicModulesService, DynamicModulesService>();
        services.AddScoped<DevelopmentAuthSeeder>();
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<IDataSeeder, SystemRoleSeeder>();
        services.AddScoped<IDataSeeder, SystemPermissionSeeder>();
        services.AddScoped<IDataSeeder, DefaultMetadataSeeder>();

        return services;
    }
}
