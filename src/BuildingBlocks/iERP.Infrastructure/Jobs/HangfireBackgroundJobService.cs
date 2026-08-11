using Hangfire;
using iERP.Application.Abstractions.Jobs;
using Microsoft.Extensions.Logging;

namespace iERP.Infrastructure.Jobs;

public sealed class HangfireBackgroundJobService : IBackgroundJobService
{
    private readonly ILogger<HangfireBackgroundJobService> _logger;

    public HangfireBackgroundJobService(ILogger<HangfireBackgroundJobService> logger)
    {
        _logger = logger;
    }

    public string Enqueue(ExpressionJob job)
    {
        _logger.LogInformation("Enqueue placeholder job {JobName}", job.JobName);
        return BackgroundJob.Enqueue(() => ExecutePlaceholder(job.JobName));
    }

    public string Schedule(ExpressionJob job, TimeSpan delay)
    {
        _logger.LogInformation("Schedule placeholder job {JobName} in {Delay}", job.JobName, delay);
        return BackgroundJob.Schedule(() => ExecutePlaceholder(job.JobName), delay);
    }

    public static void ExecutePlaceholder(string jobName)
    {
        // No-op placeholder for foundation.
    }
}
