namespace iERP.Application.Abstractions.Engines;

public interface IRuleEngine
{
    Task EvaluateAsync(Guid tenantId, string entityName, string eventName, object context, CancellationToken cancellationToken = default);
}
