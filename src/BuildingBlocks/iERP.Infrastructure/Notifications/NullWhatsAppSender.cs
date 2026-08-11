using iERP.Application.Abstractions.Notifications;
using Microsoft.Extensions.Logging;

namespace iERP.Infrastructure.Notifications;

public sealed class NullWhatsAppSender : IWhatsAppSender
{
    private readonly ILogger<NullWhatsAppSender> _logger;

    public NullWhatsAppSender(ILogger<NullWhatsAppSender> logger) => _logger = logger;

    public Task SendAsync(string toPhoneNumber, string message, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("NullWhatsAppSender to {To}", toPhoneNumber);
        return Task.CompletedTask;
    }
}
