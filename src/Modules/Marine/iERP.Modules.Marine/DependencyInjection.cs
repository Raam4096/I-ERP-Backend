using iERP.Infrastructure.Persistence.Interceptors;
using iERP.Modules.Marine.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace iERP.Modules.Marine;

public static class DependencyInjection
{
    public static IServiceCollection AddMarineModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PrimaryDatabase")
            ?? "Host=localhost;Port=5432;Database=ierp;Username=ierp;Password=ierp";

        services.AddDbContext<MarineDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, b => b.MigrationsAssembly("iERP.Migrations"));
            options.AddInterceptors(
                sp.GetRequiredService<TenantSaveChangesInterceptor>(),
                sp.GetRequiredService<AuditSaveChangesInterceptor>());
        });

        return services;
    }
}
