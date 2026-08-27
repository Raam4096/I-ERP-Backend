namespace iERP.Application.Abstractions.Metadata;

/// <summary>
/// Cross-module store for tenant custom field values (Hybrid screens).
/// Implemented by Platform Metadata; consumed by CRM and other modules.
/// </summary>
public interface ICustomFieldValueStore
{
    Task<IReadOnlyDictionary<string, object?>> GetValuesAsync(
        string entityName,
        Guid recordId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<Guid, IReadOnlyDictionary<string, object?>>> GetValuesForRecordsAsync(
        string entityName,
        IReadOnlyCollection<Guid> recordIds,
        CancellationToken cancellationToken = default);

    Task UpsertValuesAsync(
        string entityName,
        Guid recordId,
        IReadOnlyDictionary<string, object?> values,
        CancellationToken cancellationToken = default);
}
