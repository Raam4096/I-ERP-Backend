using iERP.Modules.Platform.Tenancy.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace iERP.Worker;

/// <summary>
/// Skeleton worker that will later publish outbox messages to Azure Service Bus.
/// </summary>
public sealed class OutboxProcessorWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxProcessorWorker> _logger;

    public OutboxProcessorWorker(IServiceScopeFactory scopeFactory, ILogger<OutboxProcessorWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox processor worker started (placeholder).");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<PlatformDbContext>();
                var pending = await db.OutboxMessages
                    .IgnoreQueryFilters()
                    .Where(x => x.ProcessedAt == null)
                    .OrderBy(x => x.OccurredAt)
                    .Take(50)
                    .ToListAsync(stoppingToken);

                if (pending.Count > 0)
                {
                    _logger.LogDebug("Found {Count} pending outbox messages (not published yet).", pending.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Outbox poll skipped (database may be unavailable).");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
