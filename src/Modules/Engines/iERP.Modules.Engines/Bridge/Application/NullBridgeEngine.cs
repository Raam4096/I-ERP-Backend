using iERP.Application.Abstractions.Engines;
using Microsoft.Extensions.Logging;

namespace iERP.Modules.Engines.Bridge.Application;

public sealed class NullBridgeEngine : IBridgeEngine
{
    private readonly ILogger<NullBridgeEngine> _logger;
    public NullBridgeEngine(ILogger<NullBridgeEngine> logger) => _logger = logger;
    public Task ConvertAsync(Guid tenantId, Guid bridgeDefinitionId, Guid sourceRecordId, Guid actedBy, CancellationToken cancellationToken = default)
    { _logger.LogDebug("Bridge convert placeholder"); return Task.CompletedTask; }
}
