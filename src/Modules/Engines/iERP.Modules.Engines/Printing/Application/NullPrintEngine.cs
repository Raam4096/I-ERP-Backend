using iERP.Application.Abstractions.Engines;
using Microsoft.Extensions.Logging;

namespace iERP.Modules.Engines.Printing.Application;

public sealed class NullPrintEngine : IPrintEngine
{
    private readonly ILogger<NullPrintEngine> _logger;
    public NullPrintEngine(ILogger<NullPrintEngine> logger) => _logger = logger;
    public Task<byte[]> RenderAsync(Guid tenantId, string entityName, Guid recordId, string templateCode, CancellationToken cancellationToken = default)
    { _logger.LogDebug("Print render placeholder"); return Task.FromResult(Array.Empty<byte>()); }
}
