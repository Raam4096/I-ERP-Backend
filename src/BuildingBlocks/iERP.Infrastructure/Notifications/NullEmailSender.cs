using iERP.Application.Abstractions.Notifications;
using Microsoft.Extensions.Logging;

namespace iERP.Infrastructure.Notifications;

public sealed class NullEmailSender : IEmailSender
{
    private readonly ILogger<NullEmailSender> _logger;

    public NullEmailSender(ILogger<NullEmailSender> logger) => _logger = logger;

    public Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("NullEmailSender to {To}: {Subject}", to, subject);
        return Task.CompletedTask;
    }
}
