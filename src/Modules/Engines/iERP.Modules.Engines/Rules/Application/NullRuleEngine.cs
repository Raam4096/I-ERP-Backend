using iERP.Application.Abstractions.Engines;
using Microsoft.Extensions.Logging;

namespace iERP.Modules.Engines.Rules.Application;

public sealed class NullRuleEngine : IRuleEngine
{
    private readonly ILogger<NullRuleEngine> _logger;
    public NullRuleEngine(ILogger<NullRuleEngine> logger) => _logger = logger;
    public Task EvaluateAsync(Guid tenantId, string entityName, string eventName, object context, CancellationToken cancellationToken = default)
    { _logger.LogDebug("Rule evaluate placeholder"); return Task.CompletedTask; }
}
