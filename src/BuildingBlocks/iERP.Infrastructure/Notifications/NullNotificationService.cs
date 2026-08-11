using iERP.Application.Abstractions.Notifications;
using Microsoft.Extensions.Logging;

namespace iERP.Infrastructure.Notifications;

public sealed class NullNotificationService : INotificationService
{
    private readonly ILogger<NullNotificationService> _logger;

    public NullNotificationService(ILogger<NullNotificationService> logger) => _logger = logger;

    public Task NotifyAsync(
        Guid tenantId,
        Guid? userId,
        string channel,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("NullNotificationService {Channel} tenant {TenantId}", channel, tenantId);
        return Task.CompletedTask;
    }
}
