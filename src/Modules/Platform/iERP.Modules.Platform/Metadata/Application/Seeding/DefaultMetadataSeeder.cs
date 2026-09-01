using iERP.Application.Abstractions.Metadata;
using iERP.Application.Abstractions.Seeding;
using iERP.Modules.Platform.Metadata.Domain;
using iERP.Modules.Platform.Metadata.Infrastructure;
using iERP.Modules.Platform.Tenancy.Infrastructure;
using iERP.SharedKernel.Tenancy;
using iERP.SharedKernel.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace iERP.Modules.Platform.Metadata.Application.Seeding;

/// <summary>
/// Seeds all predefined product modules/screens for every tenant.
/// CRM is strict: display name "CRM", screens = Leads + Opportunities only.
/// Extra CRM screens previously seeded are soft-deleted. Other modules stay stubs
/// (<c>renderMode = under_implementation</c>) until implemented.
/// </summary>
public sealed class DefaultMetadataSeeder : IDataSeeder
{
    public const string CrmModuleCode = CrmLeadsScreenCatalog.ModuleCode;
    public const string CrmLeadsScreenCode = CrmLeadsScreenCatalog.ScreenCode;
    public const string CrmOpportunitiesScreenCode = CrmOpportunitiesScreenCatalog.ScreenCode;

    private readonly MetadataDbContext _db;
    private readonly PlatformDbContext _platformDb;
    private readonly ITenantContext _tenantContext;
    private readonly IClock _clock;
    private readonly ILogger<DefaultMetadataSeeder> _logger;

    public DefaultMetadataSeeder(
        MetadataDbContext db,
        PlatformDbContext platformDb,
        ITenantContext tenantContext,
        IClock clock,
        ILogger<DefaultMetadataSeeder> logger)
    {
        _db = db;
        _platformDb = platformDb;
        _tenantContext = tenantContext;
        _clock = clock;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var tenantIds = await _platformDb.Tenants
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if (tenantIds.Count == 0 && _tenantContext.HasTenant && _tenantContext.TenantId is Guid single)
        {
            tenantIds.Add(single);
        }

        if (tenantIds.Count == 0)
        {
            _logger.LogWarning(
                "DefaultMetadataSeeder: no tenants found; skipping predefined metadata. " +
                "Create a tenant (or enable AuthSeed) and restart the API.");
            return;
        }

        foreach (var tenantId in tenantIds)
        {
            _tenantContext.SetTenant(tenantId);
            await SeedTenantAsync(tenantId, cancellationToken);
        }
    }

    private async Task SeedTenantAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        var changedModules = 0;
        var changedScreens = 0;

        foreach (var moduleSpec in PredefinedModulesCatalog.Modules)
        {
            var module = await EnsureModuleAsync(tenantId, moduleSpec, cancellationToken);
            changedModules++;

            foreach (var screenSpec in moduleSpec.Screens)
            {
                if (screenSpec.Code == CrmLeadsScreenCode)
                {
                    if (await EnsureCrmLeadsLayoutAsync(tenantId, module.Id, cancellationToken))
                    {
                        changedScreens++;
                    }

                    continue;
                }

                if (await EnsureStubOrBasicScreenAsync(tenantId, module.Id, screenSpec, cancellationToken))
                {
                    changedScreens++;
                }
            }

            // Strict catalog: soft-delete screens for this module that are no longer defined.
            changedScreens += await DeactivateOrphanScreensAsync(
                tenantId,
                module.Id,
                moduleSpec.Screens.Select(s => s.Code).ToHashSet(StringComparer.OrdinalIgnoreCase),
                cancellationToken);
        }

        _logger.LogInformation(
            "Predefined metadata sync for tenant {TenantId}: {ModuleCount} modules catalogued, {ScreenChanges} screen create/update(s)",
            tenantId,
            changedModules,
            changedScreens);
    }

    private async Task<int> DeactivateOrphanScreensAsync(
        Guid tenantId,
        Guid moduleId,
        HashSet<string> allowedCodes,
        CancellationToken cancellationToken)
    {
        var orphans = await _db.ScreenDefinitions
            .IgnoreQueryFilters()
            .Include(x => x.Sections)
            .ThenInclude(s => s.Fields)
            .Where(x => x.TenantId == tenantId
                        && x.ModuleDefinitionId == moduleId
                        && !x.IsDeleted)
            .ToListAsync(cancellationToken);

        var now = _clock.UtcNow;
        var changed = 0;
        foreach (var screen in orphans.Where(s => !allowedCodes.Contains(s.Code)))
        {
            foreach (var section in screen.Sections.Where(s => !s.IsDeleted))
            {
                foreach (var field in section.Fields.Where(f => !f.IsDeleted))
                {
                    field.SoftDelete(null, now);
                }

                section.SoftDelete(null, now);
            }

            screen.SoftDelete(null, now);
            changed++;
            _logger.LogInformation(
                "Soft-deleted orphan metadata screen {ScreenCode} for tenant {TenantId} (not in predefined catalog)",
                screen.Code,
                tenantId);
        }

        if (changed > 0)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }

        return changed;
    }

    private async Task<ModuleDefinition> EnsureModuleAsync(
        Guid tenantId,
        PredefinedModuleSpec spec,
        CancellationToken cancellationToken)
    {
        var module = await _db.ModuleDefinitions
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x => x.TenantId == tenantId && x.Code == spec.Code,
                cancellationToken);

        if (module is null)
        {
            module = new ModuleDefinition
            {
                Code = spec.Code,
                Name = spec.Name,
                Description = spec.Description,
                IsActive = true
            };
            module.SetTenantId(tenantId);
            _db.ModuleDefinitions.Add(module);
            await _db.SaveChangesAsync(cancellationToken);
            return module;
        }

        var dirty = false;
        if (module.IsDeleted)
        {
            module.IsDeleted = false;
            module.DeletedAt = null;
            module.DeletedBy = null;
            dirty = true;
        }

        if (module.Name != spec.Name || module.Description != spec.Description || !module.IsActive)
        {
            module.Name = spec.Name;
            module.Description = spec.Description;
            module.IsActive = true;
            dirty = true;
        }

        if (dirty)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }

        return module;
    }

    private async Task<bool> EnsureStubOrBasicScreenAsync(
        Guid tenantId,
        Guid moduleId,
        PredefinedScreenSpec spec,
        CancellationToken cancellationToken)
    {
        var screen = await _db.ScreenDefinitions
            .IgnoreQueryFilters()
            .Include(x => x.Sections)
            .ThenInclude(s => s.Fields)
            .FirstOrDefaultAsync(
                x => x.TenantId == tenantId && x.Code == spec.Code,
                cancellationToken);

        var changed = false;
        if (screen is null)
        {
            screen = new ScreenDefinition
            {
                ModuleDefinitionId = moduleId,
                Code = spec.Code,
                Name = spec.Name,
                Route = spec.Route,
                RenderMode = spec.RenderMode,
                EntityName = spec.Code,
                ApiBasePath = spec.ApiBasePath,
                WorkflowEnabled = false,
                PrintEnabled = false,
                AiEnabled = false
            };
            screen.SetTenantId(tenantId);
            _db.ScreenDefinitions.Add(screen);
            changed = true;
        }
        else
        {
            if (screen.IsDeleted)
            {
                screen.IsDeleted = false;
                screen.DeletedAt = null;
                screen.DeletedBy = null;
                changed = true;
            }

            if (screen.ModuleDefinitionId != moduleId
                || screen.Name != spec.Name
                || screen.Route != spec.Route
                || screen.ApiBasePath != spec.ApiBasePath
                || screen.RenderMode != spec.RenderMode
                || screen.EntityName != spec.Code)
            {
                screen.ModuleDefinitionId = moduleId;
                screen.Name = spec.Name;
                screen.Route = spec.Route;
                screen.ApiBasePath = spec.ApiBasePath;
                screen.RenderMode = spec.RenderMode;
                screen.EntityName = spec.Code;
                changed = true;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);

        if (spec.IsImplemented && spec.Code == CrmOpportunitiesScreenCode)
        {
            if (await EnsureOpportunityDetailsSectionAsync(tenantId, screen, cancellationToken))
            {
                changed = true;
            }

            return changed;
        }

        // Stub screens: single empty section so UI can show "under implementation".
        if (!spec.IsImplemented)
        {
            if (await EnsureUnderImplementationSectionAsync(tenantId, screen, cancellationToken))
            {
                changed = true;
            }
        }

        return changed;
    }

    private async Task<bool> EnsureUnderImplementationSectionAsync(
        Guid tenantId,
        ScreenDefinition screen,
        CancellationToken cancellationToken)
    {
        const string sectionCode = "main";
        var section = screen.Sections.FirstOrDefault(s =>
            s.Code.Equals(sectionCode, StringComparison.OrdinalIgnoreCase));

        if (section is null)
        {
            section = new SectionDefinition
            {
                ScreenDefinitionId = screen.Id,
                Code = sectionCode,
                Name = "Details",
                Description = "This screen is under implementation. Schema and data APIs will be connected in a later release.",
                DisplayOrder = 1
            };
            section.SetTenantId(tenantId);
            screen.Sections.Add(section);
            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }

        var dirty = false;
        if (section.IsDeleted)
        {
            section.IsDeleted = false;
            section.DeletedAt = null;
            section.DeletedBy = null;
            dirty = true;
        }

        const string description =
            "This screen is under implementation. Schema and data APIs will be connected in a later release.";
        if (section.Name != "Details" || section.Description != description || section.DisplayOrder != 1)
        {
            section.Name = "Details";
            section.Description = description;
            section.DisplayOrder = 1;
            dirty = true;
        }

        if (dirty)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }

        return dirty;
    }

    private async Task<bool> EnsureOpportunityDetailsSectionAsync(
        Guid tenantId,
        ScreenDefinition screen,
        CancellationToken cancellationToken)
    {
        const string sectionCode = "main";
        var section = screen.Sections.FirstOrDefault(s =>
            s.Code.Equals(sectionCode, StringComparison.OrdinalIgnoreCase));

        var fields = new (string Key, string Label, string DataType, string Control, int Order, bool Required, bool ReadOnly)[]
        {
            ("opportunityNumber", "Opportunity Number", "text", "input", 1, false, true),
            ("name", "Name", "text", "input", 2, true, false),
            ("stage", "Stage", "text", "input", 3, false, false),
            ("opportunityValue", "Opportunity Value", "number", "number", 4, false, false),
            ("status", "Status", "text", "input", 5, false, false),
            ("notes", "Notes", "text", "textarea", 6, false, false),
        };

        var changed = false;
        if (section is null)
        {
            section = new SectionDefinition
            {
                ScreenDefinitionId = screen.Id,
                Code = sectionCode,
                Name = "Details",
                Description = "Opportunity details.",
                DisplayOrder = 1
            };
            section.SetTenantId(tenantId);
            screen.Sections.Add(section);
            changed = true;
            await _db.SaveChangesAsync(cancellationToken);
        }
        else if (section.IsDeleted)
        {
            section.IsDeleted = false;
            section.DeletedAt = null;
            section.DeletedBy = null;
            section.Description = "Opportunity details.";
            changed = true;
        }

        foreach (var f in fields)
        {
            var field = section.Fields.FirstOrDefault(x =>
                x.FieldKey.Equals(f.Key, StringComparison.OrdinalIgnoreCase));
            if (field is null)
            {
                field = new FieldDefinition
                {
                    SectionDefinitionId = section.Id,
                    FieldKey = f.Key,
                    Label = f.Label,
                    DataType = f.DataType,
                    ControlType = f.Control,
                    DisplayOrder = f.Order,
                    IsRequired = f.Required,
                    IsReadOnly = f.ReadOnly,
                    IsVisible = true,
                    Width = 3
                };
                field.SetTenantId(tenantId);
                section.Fields.Add(field);
                changed = true;
            }
            else if (field.IsDeleted)
            {
                field.IsDeleted = false;
                field.DeletedAt = null;
                field.DeletedBy = null;
                changed = true;
            }
        }

        if (changed)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }

        return changed;
    }

    private async Task<bool> EnsureCrmLeadsLayoutAsync(Guid tenantId, Guid moduleId, CancellationToken cancellationToken)
    {
        var screen = await _db.ScreenDefinitions
            .IgnoreQueryFilters()
            .Include(x => x.Sections)
            .ThenInclude(s => s.Fields)
            .FirstOrDefaultAsync(
                x => x.TenantId == tenantId && x.Code == CrmLeadsScreenCode,
                cancellationToken);

        var changed = false;
        if (screen is null)
        {
            screen = new ScreenDefinition
            {
                ModuleDefinitionId = moduleId,
                Code = CrmLeadsScreenCode,
                Name = CrmLeadsScreenCatalog.ScreenName,
                Route = CrmLeadsScreenCatalog.Route,
                RenderMode = PredefinedModulesCatalog.GenericRenderMode,
                EntityName = CrmLeadsScreenCode,
                ApiBasePath = CrmLeadsScreenCatalog.ApiBasePath,
                WorkflowEnabled = false,
                PrintEnabled = false,
                AiEnabled = true
            };
            screen.SetTenantId(tenantId);
            _db.ScreenDefinitions.Add(screen);
            changed = true;
        }
        else
        {
            if (screen.IsDeleted)
            {
                screen.IsDeleted = false;
                screen.DeletedAt = null;
                screen.DeletedBy = null;
                changed = true;
            }

            screen.ModuleDefinitionId = moduleId;
            screen.Name = CrmLeadsScreenCatalog.ScreenName;
            screen.Route = CrmLeadsScreenCatalog.Route;
            screen.ApiBasePath = CrmLeadsScreenCatalog.ApiBasePath;
            screen.EntityName = CrmLeadsScreenCode;
            screen.RenderMode = PredefinedModulesCatalog.GenericRenderMode;
        }

        await _db.SaveChangesAsync(cancellationToken);

        var catalogCodes = CrmLeadsScreenCatalog.Sections.Select(x => x.Code).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var now = _clock.UtcNow;

        foreach (var section in screen.Sections.Where(s => !s.IsDeleted && !catalogCodes.Contains(s.Code)))
        {
            foreach (var field in section.Fields.Where(f => !f.IsDeleted))
            {
                field.SoftDelete(null, now);
            }

            section.SoftDelete(null, now);
            changed = true;
        }

        foreach (var spec in CrmLeadsScreenCatalog.Sections.OrderBy(x => x.Order))
        {
            var section = screen.Sections.FirstOrDefault(s =>
                s.Code.Equals(spec.Code, StringComparison.OrdinalIgnoreCase));

            if (section is null)
            {
                section = new SectionDefinition
                {
                    ScreenDefinitionId = screen.Id,
                    Code = spec.Code,
                    Name = spec.Name,
                    Description = spec.Description,
                    DisplayOrder = spec.Order
                };
                section.SetTenantId(tenantId);
                screen.Sections.Add(section);
                _db.SectionDefinitions.Add(section);
                changed = true;
            }
            else
            {
                if (section.IsDeleted)
                {
                    section.IsDeleted = false;
                    section.DeletedAt = null;
                    section.DeletedBy = null;
                    changed = true;
                }

                if (section.Name != spec.Name
                    || section.Description != spec.Description
                    || section.DisplayOrder != spec.Order)
                {
                    section.Name = spec.Name;
                    section.Description = spec.Description;
                    section.DisplayOrder = spec.Order;
                    changed = true;
                }
            }

            await _db.SaveChangesAsync(cancellationToken);

            var specKeys = spec.Fields.Select(f => f.FieldKey).ToHashSet(StringComparer.OrdinalIgnoreCase);
            foreach (var field in section.Fields.Where(f => !f.IsDeleted && !specKeys.Contains(f.FieldKey)))
            {
                field.SoftDelete(null, now);
                changed = true;
            }

            foreach (var fieldSpec in spec.Fields.OrderBy(x => x.Order))
            {
                var field = section.Fields.FirstOrDefault(f =>
                    f.FieldKey.Equals(fieldSpec.FieldKey, StringComparison.OrdinalIgnoreCase));

                if (field is null)
                {
                    field = new FieldDefinition
                    {
                        SectionDefinitionId = section.Id,
                        FieldKey = fieldSpec.FieldKey,
                        Label = fieldSpec.Label,
                        DataType = fieldSpec.DataType,
                        ControlType = fieldSpec.ControlType,
                        DisplayOrder = fieldSpec.Order,
                        IsRequired = fieldSpec.Required,
                        IsReadOnly = fieldSpec.ReadOnly,
                        IsVisible = true,
                        Width = 3
                    };
                    field.SetTenantId(tenantId);
                    section.Fields.Add(field);
                    changed = true;
                }
                else
                {
                    if (field.IsDeleted)
                    {
                        field.IsDeleted = false;
                        field.DeletedAt = null;
                        field.DeletedBy = null;
                        changed = true;
                    }

                    field.Label = fieldSpec.Label;
                    field.DataType = fieldSpec.DataType;
                    field.ControlType = fieldSpec.ControlType;
                    field.DisplayOrder = fieldSpec.Order;
                    field.IsRequired = fieldSpec.Required;
                    field.IsReadOnly = fieldSpec.ReadOnly;
                    field.IsVisible = true;
                }
            }
        }

        if (changed || _db.ChangeTracker.HasChanges())
        {
            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }

        return false;
    }
}
