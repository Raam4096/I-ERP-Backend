using iERP.Application.Abstractions.Jobs;
using Microsoft.Extensions.Logging;

namespace iERP.Infrastructure.Jobs;

public sealed class NullBackgroundJobService : IBackgroundJobService
{
    private readonly ILogger<NullBackgroundJobService> _logger;

    public NullBackgroundJobService(ILogger<NullBackgroundJobService> logger)
    {
        _logger = logger;
    }

    public string Enqueue(ExpressionJob job)
    {
        _logger.LogDebug("NullBackgroundJobService enqueue {JobName}", job.JobName);
        return Guid.NewGuid().ToString("N");
    }

    public string Schedule(ExpressionJob job, TimeSpan delay)
    {
        _logger.LogDebug("NullBackgroundJobService schedule {JobName}", job.JobName);
        return Guid.NewGuid().ToString("N");
    }
}
