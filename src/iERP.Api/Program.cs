using Hangfire;
using Hangfire.PostgreSql;
using iERP.Api;
using iERP.Application.Abstractions.Jobs;
using iERP.Application.Abstractions.Options;
using iERP.Infrastructure;
using iERP.Infrastructure.Jobs;
using iERP.Infrastructure.Middleware;
using iERP.Infrastructure.Tenancy;
using iERP.Modules.AI;
using iERP.Modules.AI.Api;
using iERP.Modules.Assets;
using iERP.Modules.Assets.Api;
using iERP.Modules.Banking;
using iERP.Modules.Banking.Api;
using iERP.Modules.Catalog;
using iERP.Modules.Catalog.Api;
using iERP.Modules.CRM;
using iERP.Modules.CRM.Api;
using iERP.Modules.Engines;
using iERP.Modules.Engines.Api;
using iERP.Modules.Finance;
using iERP.Modules.Finance.Api;
using iERP.Modules.HR;
using iERP.Modules.HR.Api;
using iERP.Modules.Inventory;
using iERP.Modules.Inventory.Api;
using iERP.Modules.Manufacturing;
using iERP.Modules.Manufacturing.Api;
using iERP.Modules.Marine;
using iERP.Modules.Marine.Api;
using iERP.Modules.Platform;
using iERP.Modules.Platform.Api;
using iERP.Modules.Procurement;
using iERP.Modules.Procurement.Api;
using iERP.Modules.Projects;
using iERP.Modules.Projects.Api;
using iERP.Modules.Reporting;
using iERP.Modules.Reporting.Api;
using iERP.Modules.Sales;
using iERP.Modules.Sales.Api;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIerpInfrastructure(builder.Configuration);
builder.Services.AddPlatformModule(builder.Configuration);
builder.Services.AddEnginesModule(builder.Configuration);
builder.Services.AddCrmModule(builder.Configuration);
builder.Services.AddCatalogModule(builder.Configuration);
builder.Services.AddSalesModule(builder.Configuration);
builder.Services.AddProcurementModule(builder.Configuration);
builder.Services.AddInventoryModule(builder.Configuration);
builder.Services.AddFinanceModule(builder.Configuration);
builder.Services.AddBankingModule(builder.Configuration);
builder.Services.AddProjectsModule(builder.Configuration);
builder.Services.AddHrModule(builder.Configuration);
builder.Services.AddManufacturingModule(builder.Configuration);
builder.Services.AddAssetsModule(builder.Configuration);
builder.Services.AddMarineModule(builder.Configuration);
builder.Services.AddReportingModule(builder.Configuration);
builder.Services.AddAiModule(builder.Configuration);

builder.Services.AddIerpRateLimiting();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "i-ERP API", Version = "v1" });
});

var hangfireOptions = builder.Configuration.GetSection(HangfireOptions.SectionName).Get<HangfireOptions>() ?? new HangfireOptions();
var connectionString = builder.Configuration.GetConnectionString("PrimaryDatabase");
var hangfireEnabled = hangfireOptions.Enabled && !string.IsNullOrWhiteSpace(connectionString);

if (hangfireEnabled)
{
    builder.Services.AddHangfire(config => config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(connectionString)));
    builder.Services.AddHangfireServer(options => options.WorkerCount = hangfireOptions.WorkerCount);
    builder.Services.AddScoped<IBackgroundJobService, HangfireBackgroundJobService>();
}
else
{
    builder.Services.AddScoped<IBackgroundJobService, NullBackgroundJobService>();
}

var app = builder.Build();

app.UseExceptionHandler();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseMiddleware<TenantResolutionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

if (hangfireEnabled)
{
    app.UseHangfireDashboard("/hangfire");
}

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("live")
});
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("ready")
});
app.MapHealthChecks("/health");

app.MapPlatformEndpoints();
app.MapEnginesEndpoints();
app.MapCrmEndpoints();
app.MapCustomerEndpoints();
app.MapCatalogEndpoints();
app.MapSalesEndpoints();
app.MapSalesExtraEndpoints();
app.MapProcurementEndpoints();
app.MapInventoryEndpoints();
app.MapFinanceEndpoints();
app.MapBankingEndpoints();
app.MapProjectsEndpoints();
app.MapHrEndpoints();
app.MapManufacturingEndpoints();
app.MapAssetsEndpoints();
app.MapMarineEndpoints();
app.MapReportingEndpoints();
app.MapAiEndpoints();

app.Run();

public partial class Program;
