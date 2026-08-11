namespace iERP.Application.Abstractions.Engines;

public interface IBridgeEngine
{
    Task ConvertAsync(Guid tenantId, Guid bridgeDefinitionId, Guid sourceRecordId, Guid actedBy, CancellationToken cancellationToken = default);
}
