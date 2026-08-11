using Hangfire;
using Hangfire.PostgreSql;
using iERP.Application.Abstractions.Jobs;
using iERP.Application.Abstractions.Options;
using iERP.Infrastructure;
using iERP.Infrastructure.Jobs;
using iERP.Modules.Platform;
using iERP.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddIerpInfrastructure(builder.Configuration);
builder.Services.AddPlatformModule(builder.Configuration);

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

builder.Services.AddHostedService<OutboxProcessorWorker>();

var host = builder.Build();
host.Run();
