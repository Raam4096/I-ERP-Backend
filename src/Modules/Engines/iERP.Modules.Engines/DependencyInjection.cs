using iERP.Application.Abstractions.Engines;
using iERP.Infrastructure.Persistence.Interceptors;
using iERP.Modules.Engines.Bridge.Application;
using iERP.Modules.Engines.Bridge.Infrastructure;
using iERP.Modules.Engines.Printing.Application;
using iERP.Modules.Engines.Printing.Infrastructure;
using iERP.Modules.Engines.Rules.Application;
using iERP.Modules.Engines.Rules.Infrastructure;
using iERP.Modules.Engines.Workflow.Application;
using iERP.Modules.Engines.Workflow.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace iERP.Modules.Engines;

public static class DependencyInjection
{
    public static IServiceCollection AddEnginesModule(this IServiceCollection services, IConfiguration configuration)
    {
        var cs = configuration.GetConnectionString("PrimaryDatabase")
            ?? "Host=localhost;Port=5432;Database=ierp;Username=ierp;Password=ierp";

        void AddCtx<TContext>() where TContext : DbContext =>
            services.AddDbContext<TContext>((sp, options) =>
            {
                options.UseNpgsql(cs, b => b.MigrationsAssembly("iERP.Migrations"));
                options.AddInterceptors(
                    sp.GetRequiredService<TenantSaveChangesInterceptor>(),
                    sp.GetRequiredService<AuditSaveChangesInterceptor>());
            });

        AddCtx<WorkflowDbContext>();
        AddCtx<RulesDbContext>();
        AddCtx<BridgeDbContext>();
        AddCtx<PrintingDbContext>();

        services.AddScoped<IWorkflowEngine, NullWorkflowEngine>();
        services.AddScoped<IRuleEngine, NullRuleEngine>();
        services.AddScoped<IBridgeEngine, NullBridgeEngine>();
        services.AddScoped<IPrintEngine, NullPrintEngine>();
        return services;
    }
}
