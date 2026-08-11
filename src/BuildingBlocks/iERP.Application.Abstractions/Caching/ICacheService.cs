namespace iERP.Application.Abstractions.Caching;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    static string BuildKey(Guid tenantId, string module, string resource, string key) =>
        $"ierp:{tenantId}:{module}:{resource}:{key}";
}
