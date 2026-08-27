using System.Text.Json;
using System.Text.RegularExpressions;
using iERP.Modules.Platform.DynamicModules.Application.Dtos;
using iERP.Modules.Platform.DynamicModules.Domain;
using iERP.Modules.Platform.Tenancy.Infrastructure;
using iERP.SharedKernel.Exceptions;
using iERP.SharedKernel.Security;
using iERP.SharedKernel.Tenancy;
using iERP.SharedKernel.Time;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.Platform.DynamicModules.Application;

public interface IDynamicModulesService
{
    Task<IReadOnlyList<DynamicModuleDto>> ListModulesAsync(bool activeOnly, CancellationToken cancellationToken);
    Task<DynamicModuleDto> GetModuleAsync(Guid moduleId, CancellationToken cancellationToken);
    Task<DynamicModuleDto> CreateModuleAsync(CreateDynamicModuleRequest request, CancellationToken cancellationToken);
    Task<DynamicModuleDto> UpdateModuleAsync(Guid moduleId, UpdateDynamicModuleRequest request, CancellationToken cancellationToken);
    Task DeleteModuleAsync(Guid moduleId, CancellationToken cancellationToken);

    Task<DynamicEntityDto> CreateEntityAsync(Guid moduleId, CreateDynamicEntityRequest request, CancellationToken cancellationToken);
    Task<DynamicEntityDto> GetEntityAsync(Guid entityId, CancellationToken cancellationToken);
    Task<DynamicEntityDto> UpdateEntityAsync(Guid entityId, UpdateDynamicEntityRequest request, CancellationToken cancellationToken);
    Task DeleteEntityAsync(Guid entityId, CancellationToken cancellationToken);

    Task<DynamicFieldDto> CreateFieldAsync(Guid entityId, CreateDynamicFieldRequest request, CancellationToken cancellationToken);
    Task<DynamicFieldDto> UpdateFieldAsync(Guid fieldId, UpdateDynamicFieldRequest request, CancellationToken cancellationToken);
    Task DeleteFieldAsync(Guid fieldId, CancellationToken cancellationToken);

    Task<IReadOnlyList<DynamicRecordDto>> ListRecordsAsync(Guid entityId, CancellationToken cancellationToken);
    Task<DynamicRecordDto> GetRecordAsync(Guid recordId, CancellationToken cancellationToken);
    Task<DynamicRecordDto> CreateRecordAsync(Guid entityId, UpsertDynamicRecordRequest request, CancellationToken cancellationToken);
    Task<DynamicRecordDto> UpdateRecordAsync(Guid recordId, UpsertDynamicRecordRequest request, CancellationToken cancellationToken);
    Task DeleteRecordAsync(Guid recordId, CancellationToken cancellationToken);
}

public sealed class DynamicModulesService : IDynamicModulesService
{
    private static readonly HashSet<string> AllowedDataTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "string", "text", "number", "decimal", "int", "integer",
        "boolean", "bool", "date", "datetime", "email", "phone", "lookup"
    };

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private readonly PlatformDbContext _db;
    private readonly ITenantContext _tenantContext;
    private readonly ICurrentUser _currentUser;
    private readonly IClock _clock;

    public DynamicModulesService(
        PlatformDbContext db,
        ITenantContext tenantContext,
        ICurrentUser currentUser,
        IClock clock)
    {
        _db = db;
        _tenantContext = tenantContext;
        _currentUser = currentUser;
        _clock = clock;
    }

    public async Task<IReadOnlyList<DynamicModuleDto>> ListModulesAsync(bool activeOnly, CancellationToken cancellationToken)
    {
        EnsureTenant();

        var query = _db.DynamicModuleDefinitions.AsNoTracking();
        if (activeOnly)
        {
            query = query.Where(x => x.IsActive);
        }

        var modules = await query
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        var moduleIds = modules.Select(x => x.Id).ToList();
        var entities = await _db.DynamicEntityDefinitions
            .AsNoTracking()
            .Where(x => moduleIds.Contains(x.DynamicModuleDefinitionId) && (!activeOnly || x.IsActive))
            .OrderBy(x => x.DisplayName)
            .ToListAsync(cancellationToken);

        return modules.Select(m => MapModule(m, entities.Where(e => e.DynamicModuleDefinitionId == m.Id))).ToList();
    }

    public async Task<DynamicModuleDto> GetModuleAsync(Guid moduleId, CancellationToken cancellationToken)
    {
        EnsureTenant();

        var module = await _db.DynamicModuleDefinitions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == moduleId, cancellationToken)
            ?? throw new NotFoundException($"Dynamic module '{moduleId}' was not found.");

        var entities = await _db.DynamicEntityDefinitions
            .AsNoTracking()
            .Where(x => x.DynamicModuleDefinitionId == moduleId)
            .OrderBy(x => x.DisplayName)
            .ToListAsync(cancellationToken);

        return MapModule(module, entities);
    }

    public async Task<DynamicModuleDto> CreateModuleAsync(CreateDynamicModuleRequest request, CancellationToken cancellationToken)
    {
        var tenantId = EnsureTenant();
        var code = NormalizeCode(request.Code, "Module code");
        var name = RequireText(request.Name, "Module name");

        var existing = await _db.DynamicModuleDefinitions
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Code == code, cancellationToken);
        if (existing is not null)
        {
            throw new BusinessRuleException(
                ErrorCodes.DuplicateRecord,
                existing.IsDeleted
                    ? $"Module code '{code}' was used by a deleted module. Choose a different code."
                    : $"A module with code '{code}' already exists.");
        }

        var module = new DynamicModuleDefinition
        {
            Code = code,
            Name = name,
            Description = TrimOrNull(request.Description),
            IsActive = request.IsActive
        };
        module.SetTenantId(tenantId);

        _db.DynamicModuleDefinitions.Add(module);
        await _db.SaveChangesAsync(cancellationToken);

        return MapModule(module, []);
    }

    public async Task<DynamicModuleDto> UpdateModuleAsync(Guid moduleId, UpdateDynamicModuleRequest request, CancellationToken cancellationToken)
    {
        EnsureTenant();

        var module = await _db.DynamicModuleDefinitions
            .FirstOrDefaultAsync(x => x.Id == moduleId, cancellationToken)
            ?? throw new NotFoundException($"Dynamic module '{moduleId}' was not found.");

        module.Name = RequireText(request.Name, "Module name");
        module.Description = TrimOrNull(request.Description);
        module.IsActive = request.IsActive;

        await _db.SaveChangesAsync(cancellationToken);

        var entities = await _db.DynamicEntityDefinitions
            .AsNoTracking()
            .Where(x => x.DynamicModuleDefinitionId == moduleId)
            .OrderBy(x => x.DisplayName)
            .ToListAsync(cancellationToken);

        return MapModule(module, entities);
    }

    public async Task DeleteModuleAsync(Guid moduleId, CancellationToken cancellationToken)
    {
        EnsureTenant();

        var module = await _db.DynamicModuleDefinitions
            .FirstOrDefaultAsync(x => x.Id == moduleId, cancellationToken)
            ?? throw new NotFoundException($"Dynamic module '{moduleId}' was not found.");

        var now = _clock.UtcNow;
        var userId = _currentUser.UserId;

        var entities = await _db.DynamicEntityDefinitions
            .Where(x => x.DynamicModuleDefinitionId == moduleId)
            .ToListAsync(cancellationToken);
        var entityIds = entities.Select(x => x.Id).ToList();

        var fields = await _db.DynamicFieldDefinitions
            .Where(x => entityIds.Contains(x.DynamicEntityDefinitionId))
            .ToListAsync(cancellationToken);

        var records = await _db.DynamicRecords
            .Where(x => entityIds.Contains(x.DynamicEntityDefinitionId))
            .ToListAsync(cancellationToken);

        foreach (var record in records)
        {
            record.SoftDelete(userId, now);
        }

        foreach (var field in fields)
        {
            field.SoftDelete(userId, now);
        }

        foreach (var entity in entities)
        {
            entity.SoftDelete(userId, now);
        }

        module.SoftDelete(userId, now);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<DynamicEntityDto> CreateEntityAsync(Guid moduleId, CreateDynamicEntityRequest request, CancellationToken cancellationToken)
    {
        var tenantId = EnsureTenant();

        var module = await _db.DynamicModuleDefinitions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == moduleId, cancellationToken)
            ?? throw new NotFoundException($"Dynamic module '{moduleId}' was not found.");

        var entityName = NormalizeCode(request.EntityName, "Entity name");
        var displayName = RequireText(request.DisplayName, "Display name");

        var existing = await _db.DynamicEntityDefinitions
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.EntityName == entityName, cancellationToken);
        if (existing is not null)
        {
            throw new BusinessRuleException(
                ErrorCodes.DuplicateRecord,
                existing.IsDeleted
                    ? $"Entity name '{entityName}' was used by a deleted entity. Choose a different name."
                    : $"An entity with name '{entityName}' already exists.");
        }

        var entity = new DynamicEntityDefinition
        {
            DynamicModuleDefinitionId = moduleId,
            EntityName = entityName,
            DisplayName = displayName,
            IsActive = request.IsActive
        };
        entity.SetTenantId(tenantId);

        _db.DynamicEntityDefinitions.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return MapEntity(entity, module.Code, []);
    }

    public async Task<DynamicEntityDto> GetEntityAsync(Guid entityId, CancellationToken cancellationToken)
    {
        EnsureTenant();

        var entity = await _db.DynamicEntityDefinitions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == entityId, cancellationToken)
            ?? throw new NotFoundException($"Dynamic entity '{entityId}' was not found.");

        var module = await _db.DynamicModuleDefinitions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == entity.DynamicModuleDefinitionId, cancellationToken)
            ?? throw new NotFoundException($"Dynamic module '{entity.DynamicModuleDefinitionId}' was not found.");

        var fields = await _db.DynamicFieldDefinitions
            .AsNoTracking()
            .Where(x => x.DynamicEntityDefinitionId == entityId)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Label)
            .ToListAsync(cancellationToken);

        return MapEntity(entity, module.Code, fields);
    }

    public async Task<DynamicEntityDto> UpdateEntityAsync(Guid entityId, UpdateDynamicEntityRequest request, CancellationToken cancellationToken)
    {
        EnsureTenant();

        var entity = await _db.DynamicEntityDefinitions
            .FirstOrDefaultAsync(x => x.Id == entityId, cancellationToken)
            ?? throw new NotFoundException($"Dynamic entity '{entityId}' was not found.");

        var module = await _db.DynamicModuleDefinitions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == entity.DynamicModuleDefinitionId, cancellationToken)
            ?? throw new NotFoundException($"Dynamic module '{entity.DynamicModuleDefinitionId}' was not found.");

        entity.DisplayName = RequireText(request.DisplayName, "Display name");
        entity.IsActive = request.IsActive;
        await _db.SaveChangesAsync(cancellationToken);

        var fields = await _db.DynamicFieldDefinitions
            .AsNoTracking()
            .Where(x => x.DynamicEntityDefinitionId == entityId)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Label)
            .ToListAsync(cancellationToken);

        return MapEntity(entity, module.Code, fields);
    }

    public async Task DeleteEntityAsync(Guid entityId, CancellationToken cancellationToken)
    {
        EnsureTenant();

        var entity = await _db.DynamicEntityDefinitions
            .FirstOrDefaultAsync(x => x.Id == entityId, cancellationToken)
            ?? throw new NotFoundException($"Dynamic entity '{entityId}' was not found.");

        var now = _clock.UtcNow;
        var userId = _currentUser.UserId;

        var fields = await _db.DynamicFieldDefinitions
            .Where(x => x.DynamicEntityDefinitionId == entityId)
            .ToListAsync(cancellationToken);

        var records = await _db.DynamicRecords
            .Where(x => x.DynamicEntityDefinitionId == entityId)
            .ToListAsync(cancellationToken);

        foreach (var record in records)
        {
            record.SoftDelete(userId, now);
        }

        foreach (var field in fields)
        {
            field.SoftDelete(userId, now);
        }

        entity.SoftDelete(userId, now);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<DynamicFieldDto> CreateFieldAsync(Guid entityId, CreateDynamicFieldRequest request, CancellationToken cancellationToken)
    {
        var tenantId = EnsureTenant();

        _ = await _db.DynamicEntityDefinitions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == entityId, cancellationToken)
            ?? throw new NotFoundException($"Dynamic entity '{entityId}' was not found.");

        var fieldKey = NormalizeFieldKey(request.FieldKey);
        var label = RequireText(request.Label, "Field label");
        var dataType = NormalizeDataType(request.DataType);

        var existing = await _db.DynamicFieldDefinitions
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x => x.TenantId == tenantId
                     && x.DynamicEntityDefinitionId == entityId
                     && x.FieldKey == fieldKey,
                cancellationToken);
        if (existing is not null)
        {
            throw new BusinessRuleException(
                ErrorCodes.DuplicateRecord,
                existing.IsDeleted
                    ? $"Field key '{fieldKey}' was used by a deleted field. Choose a different key."
                    : $"A field with key '{fieldKey}' already exists on this entity.");
        }

        var field = new DynamicFieldDefinition
        {
            DynamicEntityDefinitionId = entityId,
            FieldKey = fieldKey,
            Label = label,
            DataType = dataType,
            DisplayOrder = request.DisplayOrder,
            IsRequired = request.IsRequired
        };
        field.SetTenantId(tenantId);

        _db.DynamicFieldDefinitions.Add(field);
        await _db.SaveChangesAsync(cancellationToken);

        return MapField(field);
    }

    public async Task<DynamicFieldDto> UpdateFieldAsync(Guid fieldId, UpdateDynamicFieldRequest request, CancellationToken cancellationToken)
    {
        EnsureTenant();

        var field = await _db.DynamicFieldDefinitions
            .FirstOrDefaultAsync(x => x.Id == fieldId, cancellationToken)
            ?? throw new NotFoundException($"Dynamic field '{fieldId}' was not found.");

        field.Label = RequireText(request.Label, "Field label");
        field.DataType = NormalizeDataType(request.DataType);
        field.DisplayOrder = request.DisplayOrder;
        field.IsRequired = request.IsRequired;

        await _db.SaveChangesAsync(cancellationToken);
        return MapField(field);
    }

    public async Task DeleteFieldAsync(Guid fieldId, CancellationToken cancellationToken)
    {
        EnsureTenant();

        var field = await _db.DynamicFieldDefinitions
            .FirstOrDefaultAsync(x => x.Id == fieldId, cancellationToken)
            ?? throw new NotFoundException($"Dynamic field '{fieldId}' was not found.");

        field.SoftDelete(_currentUser.UserId, _clock.UtcNow);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DynamicRecordDto>> ListRecordsAsync(Guid entityId, CancellationToken cancellationToken)
    {
        EnsureTenant();

        _ = await _db.DynamicEntityDefinitions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == entityId, cancellationToken)
            ?? throw new NotFoundException($"Dynamic entity '{entityId}' was not found.");

        var records = await _db.DynamicRecords
            .AsNoTracking()
            .Where(x => x.DynamicEntityDefinitionId == entityId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return records.Select(MapRecord).ToList();
    }

    public async Task<DynamicRecordDto> GetRecordAsync(Guid recordId, CancellationToken cancellationToken)
    {
        EnsureTenant();

        var record = await _db.DynamicRecords
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == recordId, cancellationToken)
            ?? throw new NotFoundException($"Dynamic record '{recordId}' was not found.");

        return MapRecord(record);
    }

    public async Task<DynamicRecordDto> CreateRecordAsync(Guid entityId, UpsertDynamicRecordRequest request, CancellationToken cancellationToken)
    {
        var tenantId = EnsureTenant();

        var entity = await _db.DynamicEntityDefinitions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == entityId, cancellationToken)
            ?? throw new NotFoundException($"Dynamic entity '{entityId}' was not found.");

        var fields = await _db.DynamicFieldDefinitions
            .AsNoTracking()
            .Where(x => x.DynamicEntityDefinitionId == entityId)
            .ToListAsync(cancellationToken);

        var payloadJson = BuildValidatedPayload(fields, request.Values);

        var record = new DynamicRecord
        {
            DynamicEntityDefinitionId = entityId,
            EntityName = entity.EntityName,
            PayloadJson = payloadJson
        };
        record.SetTenantId(tenantId);

        _db.DynamicRecords.Add(record);
        await _db.SaveChangesAsync(cancellationToken);

        return MapRecord(record);
    }

    public async Task<DynamicRecordDto> UpdateRecordAsync(Guid recordId, UpsertDynamicRecordRequest request, CancellationToken cancellationToken)
    {
        EnsureTenant();

        var record = await _db.DynamicRecords
            .FirstOrDefaultAsync(x => x.Id == recordId, cancellationToken)
            ?? throw new NotFoundException($"Dynamic record '{recordId}' was not found.");

        var fields = await _db.DynamicFieldDefinitions
            .AsNoTracking()
            .Where(x => x.DynamicEntityDefinitionId == record.DynamicEntityDefinitionId)
            .ToListAsync(cancellationToken);

        record.PayloadJson = BuildValidatedPayload(fields, request.Values);
        await _db.SaveChangesAsync(cancellationToken);

        return MapRecord(record);
    }

    public async Task DeleteRecordAsync(Guid recordId, CancellationToken cancellationToken)
    {
        EnsureTenant();

        var record = await _db.DynamicRecords
            .FirstOrDefaultAsync(x => x.Id == recordId, cancellationToken)
            ?? throw new NotFoundException($"Dynamic record '{recordId}' was not found.");

        record.SoftDelete(_currentUser.UserId, _clock.UtcNow);
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

    private static DynamicModuleDto MapModule(DynamicModuleDefinition module, IEnumerable<DynamicEntityDefinition> entities) =>
        new()
        {
            Id = module.Id,
            Code = module.Code,
            Name = module.Name,
            Description = module.Description,
            IsActive = module.IsActive,
            Entities = entities.Select(e => new DynamicEntitySummaryDto
            {
                Id = e.Id,
                EntityName = e.EntityName,
                DisplayName = e.DisplayName,
                IsActive = e.IsActive
            }).ToList()
        };

    private static DynamicEntityDto MapEntity(
        DynamicEntityDefinition entity,
        string moduleCode,
        IEnumerable<DynamicFieldDefinition> fields) =>
        new()
        {
            Id = entity.Id,
            ModuleId = entity.DynamicModuleDefinitionId,
            ModuleCode = moduleCode,
            EntityName = entity.EntityName,
            DisplayName = entity.DisplayName,
            IsActive = entity.IsActive,
            ApiBasePath = $"/api/v1/dynamic_modules/entities/{entity.Id}/records",
            Fields = fields.Select(MapField).ToList()
        };

    private static DynamicFieldDto MapField(DynamicFieldDefinition field) =>
        new()
        {
            Id = field.Id,
            EntityId = field.DynamicEntityDefinitionId,
            FieldKey = field.FieldKey,
            Label = field.Label,
            DataType = field.DataType,
            ControlType = MapControl(field.DataType),
            DisplayOrder = field.DisplayOrder,
            IsRequired = field.IsRequired
        };

    private static DynamicRecordDto MapRecord(DynamicRecord record)
    {
        using var doc = JsonDocument.Parse(string.IsNullOrWhiteSpace(record.PayloadJson) ? "{}" : record.PayloadJson);
        return new DynamicRecordDto
        {
            Id = record.Id,
            EntityId = record.DynamicEntityDefinitionId,
            EntityName = record.EntityName,
            Payload = doc.RootElement.Clone(),
            CreatedAt = record.CreatedAt,
            UpdatedAt = record.UpdatedAt
        };
    }

    private static string BuildValidatedPayload(
        IReadOnlyList<DynamicFieldDefinition> fields,
        Dictionary<string, JsonElement>? values)
    {
        values ??= new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
        var normalized = values.ToDictionary(
            x => x.Key.Trim(),
            x => x.Value,
            StringComparer.OrdinalIgnoreCase);

        var payload = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        var errors = new List<string>();

        foreach (var field in fields.OrderBy(x => x.DisplayOrder))
        {
            var present = normalized.TryGetValue(field.FieldKey, out var element);
            var isMissing = !present
                || element.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null
                || (element.ValueKind == JsonValueKind.String && string.IsNullOrWhiteSpace(element.GetString()));

            if (field.IsRequired && isMissing)
            {
                errors.Add($"Field '{field.FieldKey}' is required.");
                continue;
            }

            if (isMissing)
            {
                payload[field.FieldKey] = null;
                continue;
            }

            if (!TryCoerceValue(field.DataType, element, out var coerced, out var typeError))
            {
                errors.Add($"Field '{field.FieldKey}': {typeError}");
                continue;
            }

            payload[field.FieldKey] = coerced;
        }

        if (errors.Count > 0)
        {
            throw new ValidationException(string.Join(" ", errors));
        }

        return JsonSerializer.Serialize(payload, JsonOptions);
    }

    private static bool TryCoerceValue(string dataType, JsonElement element, out object? value, out string error)
    {
        value = null;
        error = string.Empty;
        var type = dataType.ToLowerInvariant();

        try
        {
            switch (type)
            {
                case "number":
                case "decimal":
                    if (element.ValueKind == JsonValueKind.Number && element.TryGetDecimal(out var dec))
                    {
                        value = dec;
                        return true;
                    }

                    if (element.ValueKind == JsonValueKind.String && decimal.TryParse(element.GetString(), out dec))
                    {
                        value = dec;
                        return true;
                    }

                    error = "must be a number.";
                    return false;

                case "int":
                case "integer":
                    if (element.ValueKind == JsonValueKind.Number && element.TryGetInt64(out var i64))
                    {
                        value = i64;
                        return true;
                    }

                    if (element.ValueKind == JsonValueKind.String && long.TryParse(element.GetString(), out i64))
                    {
                        value = i64;
                        return true;
                    }

                    error = "must be an integer.";
                    return false;

                case "boolean":
                case "bool":
                    if (element.ValueKind is JsonValueKind.True or JsonValueKind.False)
                    {
                        value = element.GetBoolean();
                        return true;
                    }

                    if (element.ValueKind == JsonValueKind.String && bool.TryParse(element.GetString(), out var b))
                    {
                        value = b;
                        return true;
                    }

                    error = "must be a boolean.";
                    return false;

                case "date":
                case "datetime":
                    if (element.ValueKind == JsonValueKind.String
                        && DateTimeOffset.TryParse(element.GetString(), out var dto))
                    {
                        value = dto.ToString("O");
                        return true;
                    }

                    error = "must be a valid date/datetime.";
                    return false;

                default:
                    value = element.ValueKind switch
                    {
                        JsonValueKind.String => element.GetString(),
                        JsonValueKind.Number => element.ToString(),
                        JsonValueKind.True => "true",
                        JsonValueKind.False => "false",
                        _ => element.GetRawText()
                    };
                    return true;
            }
        }
        catch
        {
            error = $"invalid value for type '{dataType}'.";
            return false;
        }
    }

    private static string MapControl(string dataType) =>
        dataType.ToLowerInvariant() switch
        {
            "number" or "decimal" or "int" or "integer" => "number",
            "date" or "datetime" => "datepicker",
            "boolean" or "bool" => "checkbox",
            "lookup" => "select",
            "text" => "textarea",
            _ => "input"
        };

    private static string NormalizeCode(string? value, string fieldName)
    {
        var raw = RequireText(value, fieldName).ToLowerInvariant();
        var normalized = Regex.Replace(raw, @"[^a-z0-9]+", "-").Trim('-');
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ValidationException($"{fieldName} must contain letters or numbers.");
        }

        return normalized;
    }

    private static string NormalizeFieldKey(string? value)
    {
        var raw = RequireText(value, "Field key");
        var normalized = Regex.Replace(raw.Trim(), @"[^A-Za-z0-9_]+", "_").Trim('_');
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ValidationException("Field key must contain letters, numbers, or underscores.");
        }

        return normalized;
    }

    private static string NormalizeDataType(string? value)
    {
        var dataType = string.IsNullOrWhiteSpace(value) ? "string" : value.Trim().ToLowerInvariant();
        if (!AllowedDataTypes.Contains(dataType))
        {
            throw new ValidationException(
                $"Unsupported data type '{dataType}'. Allowed: {string.Join(", ", AllowedDataTypes.OrderBy(x => x))}.");
        }

        return dataType;
    }

    private static string RequireText(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ValidationException($"{fieldName} is required.");
        }

        return value.Trim();
    }

    private static string? TrimOrNull(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
