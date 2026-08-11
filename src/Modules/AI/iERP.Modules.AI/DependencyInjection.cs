using iERP.Infrastructure.Persistence.Interceptors;
using iERP.Modules.AI.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace iERP.Modules.AI;

public static class DependencyInjection
{
    public static IServiceCollection AddAiModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PrimaryDatabase")
            ?? "Host=localhost;Port=5432;Database=ierp;Username=ierp;Password=ierp";

        services.AddDbContext<AiDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, b => b.MigrationsAssembly("iERP.Migrations"));
            options.AddInterceptors(
                sp.GetRequiredService<TenantSaveChangesInterceptor>(),
                sp.GetRequiredService<AuditSaveChangesInterceptor>());
        });

        services.AddSingleton<iERP.Application.Abstractions.AI.IAIToolRegistry, Application.AIToolRegistry>();
        services.AddScoped<iERP.Application.Abstractions.AI.IAIGovernanceService, Application.NullAIGovernanceService>();
        services.AddScoped<iERP.Application.Abstractions.AI.IAIOrchestrator, Application.NullAIOrchestrator>();

        return services;
    }
}
