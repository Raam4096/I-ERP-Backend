using FluentValidation;
using iERP.Infrastructure.Persistence.Interceptors;
using iERP.Modules.CRM.Application.Common;
using iERP.Modules.CRM.Application.Leads.Services;
using iERP.Modules.CRM.Application.Mapping;
using iERP.Modules.CRM.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace iERP.Modules.CRM;

public static class DependencyInjection
{
    public static IServiceCollection AddCrmModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PrimaryDatabase")
            ?? "Host=localhost;Port=5432;Database=ierp_dev;Username=ierp;Password=ierp";

        services.AddDbContext<CrmDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, b => b.MigrationsAssembly("iERP.Migrations"));
            options.AddInterceptors(
                sp.GetRequiredService<TenantSaveChangesInterceptor>(),
                sp.GetRequiredService<AuditSaveChangesInterceptor>());
        });

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(CrmMappingProfile).Assembly));
        services.AddScoped<ILeadNumberGenerator, LeadNumberGenerator>();

        return services;
    }
}
