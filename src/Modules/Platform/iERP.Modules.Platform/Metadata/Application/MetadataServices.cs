using System.Text.RegularExpressions;
using iERP.Modules.Platform.Metadata.Application.Dtos;
using iERP.Modules.Platform.Metadata.Application.Layout;
using iERP.Modules.Platform.Metadata.Domain;
using iERP.Modules.Platform.Metadata.Infrastructure;
using iERP.Modules.Platform.Tenancy.Infrastructure;
using iERP.SharedKernel.Exceptions;
using iERP.SharedKernel.Security;
using iERP.SharedKernel.Tenancy;
using iERP.SharedKernel.Time;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.Platform.Metadata.Application;

public interface IMetadataCatalogService
{
    Task<IReadOnlyList<MetadataModuleDto>> ListModulesAsync(bool activeOnly, CancellationToken cancellationToken);
}

public interface IUserFieldPreferenceService
{
    Task<IReadOnlyDictionary<string, FieldPreferenceValue>> GetPreferencesAsync(
        string screenCode,
        CancellationToken cancellationToken);

    Task SavePreferencesAsync(
        string screenCode,
        SaveScreenFieldPreferencesRequest request,
        CancellationToken cancellationToken);
}

public interface ICustomFieldDefinitionService
{
    Task<IReadOnlyList<CustomFieldDefinitionDto>> ListAsync(string entityName, CancellationToken cancellationToken);
    Task<CustomFieldDefinitionDto> CreateAsync(string entityName, CreateCustomFieldDefinitionRequest request, CancellationToken cancellationToken);
    Task<CustomFieldDefinitionDto> UpdateAsync(Guid id, UpdateCustomFieldDefinitionRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}

public sealed class MetadataCatalogService : IMetadataCatalogService
{
    private readonly MetadataDbContext _metadataDb;
    private readonly PlatformDbContext _platformDb;
    private readonly ITenantContext _tenantContext;

    public MetadataCatalogService(
        MetadataDbContext metadataDb,
        PlatformDbContext platformDb,
        ITenantContext tenantContext)
    {
        _metadataDb = metadataDb;
        _platformDb = platformDb;
        _tenantContext = tenantContext;
    }

    public async Task<IReadOnlyList<MetadataModuleDto>> ListModulesAsync(bool activeOnly, CancellationToken cancellationToken)
    {
        EnsureTenant();

        var metadataModules = await _metadataDb.ModuleDefinitions
            .AsNoTracking()
            .Include(x => x.Screens)
            .Where(x => !activeOnly || x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        var result = metadataModules.Select(m => new MetadataModuleDto
        {
            Id = m.Id,
            Code = m.Code,
            Name = m.Name,
            Description = m.Description,
            IsActive = m.IsActive,
            Source = "metadata",
            Screens = m.Screens
                .Where(s => !s.IsDeleted)
                .OrderBy(s => s.Name)
                .Select(s => new MetadataScreenSummaryDto
                {
                    Id = s.Id,
                    Code = s.Code,
                    Name = s.Name,
                    Route = s.Route,
                    EntityName = s.EntityName,
                    ApiBasePath = s.ApiBasePath
                })
                .ToList()
        }).ToList();

        var dynamicModules = await _platformDb.DynamicModuleDefinitions
            .AsNoTracking()
            .Where(x => !activeOnly || x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        var dynamicModuleIds = dynamicModules.Select(x => x.Id).ToList();
        var dynamicEntities = await _platformDb.DynamicEntityDefinitions
            .AsNoTracking()
            .Where(x => dynamicModuleIds.Contains(x.DynamicModuleDefinitionId) && (!activeOnly || x.IsActive))
            .OrderBy(x => x.DisplayName)
            .ToListAsync(cancellationToken);

        foreach (var module in dynamicModules)
        {
            result.Add(new MetadataModuleDto
            {
                Id = module.Id,
                Code = module.Code,
                Name = module.Name,
                Description = module.Description,
                IsActive = module.IsActive,
                Source = "dynamic",
                Screens = dynamicEntities
                    .Where(e => e.DynamicModuleDefinitionId == module.Id)
                    .Select(e => new MetadataScreenSummaryDto
                    {
                        Id = e.Id,
                        Code = e.EntityName,
                        Name = e.DisplayName,
                        Route = $"/dynamic/{module.Code}/{e.EntityName}",
                        EntityName = e.EntityName,
                        ApiBasePath = $"/api/v1/dynamic_modules/entities/{e.Id}/records"
                    })
                    .ToList()
            });
        }

        return result
            .OrderBy(x => x.Name)
            .ToList();
    }

    private void EnsureTenant()
    {
        if (!_tenantContext.HasTenant)
        {
            throw new ForbiddenException("Tenant context is required.", ErrorCodes.TenantNotFound);
        }
    }
}

public sealed class UserFieldPreferenceService : IUserFieldPreferenceService
{
    private readonly MetadataDbContext _db;
    private readonly PlatformDbContext _platformDb;
    private readonly ITenantContext _tenantContext;
    private readonly ICurrentUser _currentUser;
    private readonly IClock _clock;

    public UserFieldPreferenceService(
        MetadataDbContext db,
        PlatformDbContext platformDb,
        ITenantContext tenantContext,
        ICurrentUser currentUser,
        IClock clock)
    {
        _db = db;
        _platformDb = platformDb;
        _tenantContext = tenantContext;
        _currentUser = currentUser;
        _clock = clock;
    }

    public async Task<IReadOnlyDictionary<string, FieldPreferenceValue>> GetPreferencesAsync(
        string screenCode,
        CancellationToken cancellationToken)
    {
        EnsureTenant();
        var userId = RequireUserId();
        var code = screenCode.Trim();

        var rows = await _db.UserFieldPreferences
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.ScreenCode == code)
            .ToListAsync(cancellationToken);

        return rows.ToDictionary(
            x => x.FieldKey,
            x => new FieldPreferenceValue(x.FieldKey, x.IsVisible, x.DisplayOrder),
            StringComparer.OrdinalIgnoreCase);
    }

    public async Task SavePreferencesAsync(
        string screenCode,
        SaveScreenFieldPreferencesRequest request,
        CancellationToken cancellationToken)
    {
        var tenantId = EnsureTenant();
        var userId = RequireUserId();
        var code = screenCode.Trim();
        var items = (request.Fields ?? [])
            .Where(x => !string.IsNullOrWhiteSpace(x.FieldKey))
            .Select(x => new FieldPreferenceValue(x.FieldKey.Trim(), x.IsVisible, x.DisplayOrder))
            .GroupBy(x => x.FieldKey, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.Last())
            .ToList();

        var catalog = await ResolveFieldCatalogAsync(code, cancellationToken);
        if (catalog.Count == 0)
        {
            throw new NotFoundException($"Screen '{code}' was not found.");
        }

        var knownKeys = catalog.Keys.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var unknown = items.Where(x => !knownKeys.Contains(x.FieldKey)).Select(x => x.FieldKey).ToList();
        if (unknown.Count > 0)
        {
            throw new ValidationException($"Unknown field key(s): {string.Join(", ", unknown)}.");
        }

        FieldLayoutPreferenceApplier.EnsureRequiredRemainVisible(
            catalog.Select(x => (x.Key, x.Value)),
            items);

        var existing = await _db.UserFieldPreferences
            .Where(x => x.UserId == userId && x.ScreenCode == code)
            .ToListAsync(cancellationToken);

        var existingByKey = existing.ToDictionary(x => x.FieldKey, StringComparer.OrdinalIgnoreCase);
        var incomingKeys = items.Select(x => x.FieldKey).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var now = _clock.UtcNow;

        foreach (var item in items)
        {
            if (existingByKey.TryGetValue(item.FieldKey, out var row))
            {
                row.IsVisible = item.IsVisible;
                row.DisplayOrder = item.DisplayOrder;
            }
            else
            {
                var created = new UserFieldPreference
                {
                    UserId = userId,
                    ScreenCode = code,
                    FieldKey = item.FieldKey,
                    IsVisible = item.IsVisible,
                    DisplayOrder = item.DisplayOrder
                };
                created.SetTenantId(tenantId);
                _db.UserFieldPreferences.Add(created);
            }
        }

        foreach (var row in existing.Where(x => !incomingKeys.Contains(x.FieldKey)))
        {
            row.SoftDelete(userId, now);
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task<Dictionary<string, bool>> ResolveFieldCatalogAsync(string screenCode, CancellationToken cancellationToken)
    {
        var screen = await _db.ScreenDefinitions
            .AsNoTracking()
            .Include(x => x.Sections)
            .ThenInclude(s => s.Fields)
            .FirstOrDefaultAsync(x => x.Code == screenCode, cancellationToken);

        if (screen is not null)
        {
            var catalog = screen.Sections
                .SelectMany(s => s.Fields)
                .ToDictionary(f => f.FieldKey, f => f.IsRequired, StringComparer.OrdinalIgnoreCase);

            var entityKey = string.IsNullOrWhiteSpace(screen.EntityName) ? screen.Code : screen.EntityName;
            var custom = await _db.CustomFieldDefinitions
                .AsNoTracking()
                .Where(x => x.IsActive && (x.EntityName == screen.Code || x.EntityName == entityKey))
                .ToListAsync(cancellationToken);

            foreach (var field in custom)
            {
                catalog[field.FieldKey] = field.IsRequired;
            }

            return catalog;
        }

        var dynamicEntity = await _platformDb.DynamicEntityDefinitions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.EntityName == screenCode, cancellationToken);

        if (dynamicEntity is null)
        {
            return new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        }

        var dynamicFields = await _platformDb.DynamicFieldDefinitions
            .AsNoTracking()
            .Where(x => x.DynamicEntityDefinitionId == dynamicEntity.Id)
            .ToListAsync(cancellationToken);

        return dynamicFields.ToDictionary(x => x.FieldKey, x => x.IsRequired, StringComparer.OrdinalIgnoreCase);
    }

    private Guid EnsureTenant()
    {
        if (!_tenantContext.HasTenant || _tenantContext.TenantId is null)
        {
            throw new ForbiddenException("Tenant context is required.", ErrorCodes.TenantNotFound);
        }

        return _tenantContext.TenantId.Value;
    }

    private Guid RequireUserId()
    {
        if (_currentUser.UserId is null)
        {
            throw new UnauthorizedException("Authenticated user is required.");
        }

        return _currentUser.UserId.Value;
    }
}

public sealed class CustomFieldDefinitionService : ICustomFieldDefinitionService
{
    private static readonly HashSet<string> AllowedDataTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "string", "text", "number", "decimal", "int", "integer",
        "boolean", "bool", "date", "datetime", "email", "phone", "lookup"
    };

    private readonly MetadataDbContext _db;
    private readonly ITenantContext _tenantContext;
    private readonly ICurrentUser _currentUser;
    private readonly IClock _clock;

    public CustomFieldDefinitionService(
        MetadataDbContext db,
        ITenantContext tenantContext,
        ICurrentUser currentUser,
        IClock clock)
    {
        _db = db;
        _tenantContext = tenantContext;
        _currentUser = currentUser;
        _clock = clock;
    }

    public async Task<IReadOnlyList<CustomFieldDefinitionDto>> ListAsync(string entityName, CancellationToken cancellationToken)
    {
        EnsureTenant();
        var key = entityName.Trim();

        var rows = await _db.CustomFieldDefinitions
            .AsNoTracking()
            .Where(x => x.EntityName == key)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Label)
            .ToListAsync(cancellationToken);

        return rows.Select(Map).ToList();
    }

    public async Task<CustomFieldDefinitionDto> CreateAsync(
        string entityName,
        CreateCustomFieldDefinitionRequest request,
        CancellationToken cancellationToken)
    {
        var tenantId = EnsureTenant();
        var key = RequireText(entityName, "Entity name");
        var fieldKey = NormalizeFieldKey(request.FieldKey);
        var label = RequireText(request.Label, "Label");
        var dataType = NormalizeDataType(request.DataType);

        var existing = await _db.CustomFieldDefinitions
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x => x.TenantId == tenantId && x.EntityName == key && x.FieldKey == fieldKey,
                cancellationToken);
        if (existing is not null)
        {
            throw new BusinessRuleException(
                ErrorCodes.DuplicateRecord,
                existing.IsDeleted
                    ? $"Custom field '{fieldKey}' was used previously. Choose a different key."
                    : $"Custom field '{fieldKey}' already exists for '{key}'.");
        }

        var entity = new CustomFieldDefinition
        {
            EntityName = key,
            FieldKey = fieldKey,
            Label = label,
            DataType = dataType,
            DisplayOrder = request.DisplayOrder,
            IsRequired = request.IsRequired,
            IsActive = request.IsActive
        };
        entity.SetTenantId(tenantId);

        _db.CustomFieldDefinitions.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    public async Task<CustomFieldDefinitionDto> UpdateAsync(
        Guid id,
        UpdateCustomFieldDefinitionRequest request,
        CancellationToken cancellationToken)
    {
        EnsureTenant();

        var entity = await _db.CustomFieldDefinitions
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Custom field '{id}' was not found.");

        entity.Label = RequireText(request.Label, "Label");
        entity.DataType = NormalizeDataType(request.DataType);
        entity.DisplayOrder = request.DisplayOrder;
        entity.IsRequired = request.IsRequired;
        entity.IsActive = request.IsActive;

        await _db.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        EnsureTenant();

        var entity = await _db.CustomFieldDefinitions
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Custom field '{id}' was not found.");

        entity.SoftDelete(_currentUser.UserId, _clock.UtcNow);
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

    private static CustomFieldDefinitionDto Map(CustomFieldDefinition entity) =>
        new()
        {
            Id = entity.Id,
            EntityName = entity.EntityName,
            FieldKey = entity.FieldKey,
            Label = entity.Label,
            DataType = entity.DataType,
            DisplayOrder = entity.DisplayOrder,
            IsRequired = entity.IsRequired,
            IsActive = entity.IsActive
        };

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
}
