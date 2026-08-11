namespace iERP.Application.Abstractions.Engines;

public interface IPrintEngine
{
    Task<byte[]> RenderAsync(Guid tenantId, string entityName, Guid recordId, string templateCode, CancellationToken cancellationToken = default);
}
