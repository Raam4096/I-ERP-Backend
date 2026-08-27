using System.Text.Json;
using iERP.Application.Abstractions.Metadata;
using iERP.Modules.Platform.Metadata.Domain;
using iERP.Modules.Platform.Metadata.Infrastructure;
using iERP.SharedKernel.Exceptions;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.Platform.Metadata.Application;

public sealed class CustomFieldValueStore : ICustomFieldValueStore
{
    private readonly MetadataDbContext _db;
    private readonly ITenantContext _tenantContext;

    public CustomFieldValueStore(MetadataDbContext db, ITenantContext tenantContext)
    {
        _db = db;
        _tenantContext = tenantContext;
    }

    public async Task<IReadOnlyDictionary<string, object?>> GetValuesAsync(
        string entityName,
        Guid recordId,
        CancellationToken cancellationToken = default)
    {
        EnsureTenant();
        var key = NormalizeEntityName(entityName);

        var rows = await _db.CustomFieldValues
            .AsNoTracking()
            .Where(x => x.EntityName == key && x.RecordId == recordId)
            .ToListAsync(cancellationToken);

        return rows.ToDictionary(x => x.FieldKey, MapValue, StringComparer.OrdinalIgnoreCase);
    }

    public async Task<IReadOnlyDictionary<Guid, IReadOnlyDictionary<string, object?>>> GetValuesForRecordsAsync(
        string entityName,
        IReadOnlyCollection<Guid> recordIds,
        CancellationToken cancellationToken = default)
    {
        EnsureTenant();
        if (recordIds.Count == 0)
        {
            return new Dictionary<Guid, IReadOnlyDictionary<string, object?>>();
        }

        var key = NormalizeEntityName(entityName);
        var rows = await _db.CustomFieldValues
            .AsNoTracking()
            .Where(x => x.EntityName == key && recordIds.Contains(x.RecordId))
            .ToListAsync(cancellationToken);

        return rows
            .GroupBy(x => x.RecordId)
            .ToDictionary(
                g => g.Key,
                g => (IReadOnlyDictionary<string, object?>)g.ToDictionary(
                    x => x.FieldKey,
                    MapValue,
                    StringComparer.OrdinalIgnoreCase));
    }

    public async Task UpsertValuesAsync(
        string entityName,
        Guid recordId,
        IReadOnlyDictionary<string, object?> values,
        CancellationToken cancellationToken = default)
    {
        var tenantId = EnsureTenant();
        if (values.Count == 0)
        {
            return;
        }

        var key = NormalizeEntityName(entityName);
        var fieldKeys = values.Keys.Select(x => x.Trim()).Where(x => x.Length > 0).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

        var definitions = await _db.CustomFieldDefinitions
            .AsNoTracking()
            .Where(x => x.EntityName == key && x.IsActive && fieldKeys.Contains(x.FieldKey))
            .ToListAsync(cancellationToken);

        var defByKey = definitions.ToDictionary(x => x.FieldKey, StringComparer.OrdinalIgnoreCase);
        foreach (var fieldKey in fieldKeys)
        {
            if (!defByKey.ContainsKey(fieldKey))
            {
                throw new ValidationException($"Unknown or inactive custom field '{fieldKey}' for entity '{key}'.");
            }
        }

        var existing = await _db.CustomFieldValues
            .Where(x => x.EntityName == key && x.RecordId == recordId && fieldKeys.Contains(x.FieldKey))
            .ToListAsync(cancellationToken);

        var existingByKey = existing.ToDictionary(x => x.FieldKey, StringComparer.OrdinalIgnoreCase);

        foreach (var (rawKey, rawValue) in values)
        {
            var fieldKey = rawKey.Trim();
            var def = defByKey[fieldKey];
            ApplyTypedValue(def.DataType, rawValue, out var text, out var number, out var date, out var boolean, out var json);

            if (existingByKey.TryGetValue(fieldKey, out var row))
            {
                row.ValueText = text;
                row.ValueNumber = number;
                row.ValueDate = date;
                row.ValueBoolean = boolean;
                row.ValueJson = json;
            }
            else
            {
                var created = new CustomFieldValue
                {
                    EntityName = key,
                    RecordId = recordId,
                    FieldKey = fieldKey,
                    ValueText = text,
                    ValueNumber = number,
                    ValueDate = date,
                    ValueBoolean = boolean,
                    ValueJson = json
                };
                created.SetTenantId(tenantId);
                _db.CustomFieldValues.Add(created);
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    private Guid EnsureTenant()
    {
        if (!_tenantContext.HasTenant || _tenantContext.TenantId is null)
        {
            throw new ForbiddenException("Tenant context is required.", ErrorCodes.TenantNotFound);
        }

        return _tenantContext.TenantId.Value;
    }

    private static string NormalizeEntityName(string entityName)
    {
        if (string.IsNullOrWhiteSpace(entityName))
        {
            throw new ValidationException("Entity name is required.");
        }

        return entityName.Trim();
    }

    private static object? MapValue(CustomFieldValue row)
    {
        if (row.ValueBoolean is not null)
        {
            return row.ValueBoolean;
        }

        if (row.ValueNumber is not null)
        {
            return row.ValueNumber;
        }

        if (row.ValueDate is not null)
        {
            return row.ValueDate;
        }

        if (!string.IsNullOrWhiteSpace(row.ValueJson))
        {
            try
            {
                return JsonSerializer.Deserialize<JsonElement>(row.ValueJson);
            }
            catch
            {
                return row.ValueJson;
            }
        }

        return row.ValueText;
    }

    private static void ApplyTypedValue(
        string dataType,
        object? raw,
        out string? text,
        out decimal? number,
        out DateTimeOffset? date,
        out bool? boolean,
        out string? json)
    {
        text = null;
        number = null;
        date = null;
        boolean = null;
        json = null;

        if (raw is null)
        {
            return;
        }

        if (raw is JsonElement element)
        {
            raw = element.ValueKind switch
            {
                JsonValueKind.String => element.GetString(),
                JsonValueKind.Number => element.TryGetDecimal(out var d) ? d : element.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null,
                _ => element.GetRawText()
            };
        }

        if (raw is null)
        {
            return;
        }

        var type = dataType.ToLowerInvariant();
        switch (type)
        {
            case "number":
            case "decimal":
            case "int":
            case "integer":
                number = Convert.ToDecimal(raw);
                break;
            case "boolean":
            case "bool":
                boolean = Convert.ToBoolean(raw);
                break;
            case "date":
            case "datetime":
                date = raw switch
                {
                    DateTimeOffset dto => dto,
                    DateTime dt => new DateTimeOffset(DateTime.SpecifyKind(dt, DateTimeKind.Utc)),
                    string s when DateTimeOffset.TryParse(s, out var parsed) => parsed,
                    _ => throw new ValidationException($"Invalid date value for type '{dataType}'.")
                };
                break;
            default:
                text = Convert.ToString(raw);
                break;
        }
    }
}
