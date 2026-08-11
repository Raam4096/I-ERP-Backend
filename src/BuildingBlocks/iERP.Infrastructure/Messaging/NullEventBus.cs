using iERP.Application.Abstractions.Messaging;
using iERP.SharedKernel.Messaging;
using Microsoft.Extensions.Logging;

namespace iERP.Infrastructure.Messaging;

public sealed class NullEventBus : IEventBus, IIntegrationEventPublisher
{
    private readonly ILogger<NullEventBus> _logger;

    public NullEventBus(ILogger<NullEventBus> logger)
    {
        _logger = logger;
    }

    public Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent
    {
        _logger.LogDebug("NullEventBus ignored event {EventType}", integrationEvent.EventType);
        return Task.CompletedTask;
    }

    public Task PublishAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("NullEventBus ignored event {EventType}", integrationEvent.EventType);
        return Task.CompletedTask;
    }
}
