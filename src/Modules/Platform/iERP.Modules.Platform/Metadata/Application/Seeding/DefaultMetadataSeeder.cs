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
/// Seeds predefined Hybrid module/screen/section/field metadata for every tenant.
/// Keeps CRM Leads layout in sync with <see cref="CrmLeadsScreenCatalog"/> (UI section shape).
/// </summary>
public sealed class DefaultMetadataSeeder : IDataSeeder
{
    public const string CrmModuleCode = CrmLeadsScreenCatalog.ModuleCode;
    public const string CrmLeadsScreenCode = CrmLeadsScreenCatalog.ScreenCode;
    public const string CrmOpportunitiesScreenCode = "crm-opportunities";

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
        var module = await EnsureModuleAsync(tenantId, cancellationToken);
        var leadsChanged = await EnsureCrmLeadsLayoutAsync(tenantId, module.Id, cancellationToken);
        var oppsSeeded = await EnsureOpportunityScreenAsync(tenantId, module.Id, cancellationToken);

        if (leadsChanged || oppsSeeded)
        {
            _logger.LogInformation(
                "Synced predefined CRM metadata for tenant {TenantId} (leadsLayout={Leads}, opportunities={Opps})",
                tenantId,
                leadsChanged,
                oppsSeeded);
        }
    }

    private async Task<ModuleDefinition> EnsureModuleAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        var module = await _db.ModuleDefinitions
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x => x.TenantId == tenantId && x.Code == CrmModuleCode && !x.IsDeleted,
                cancellationToken);

        if (module is not null)
        {
            return module;
        }

        module = await _db.ModuleDefinitions
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x => x.TenantId == tenantId && x.Code == CrmModuleCode,
                cancellationToken);

        if (module is not null)
        {
            module.IsDeleted = false;
            module.DeletedAt = null;
            module.DeletedBy = null;
            module.IsActive = true;
            module.Name = "CRM";
            await _db.SaveChangesAsync(cancellationToken);
            return module;
        }

        module = new ModuleDefinition
        {
            Code = CrmModuleCode,
            Name = "CRM",
            Description = "Customer relationship management",
            IsActive = true
        };
        module.SetTenantId(tenantId);
        _db.ModuleDefinitions.Add(module);
        await _db.SaveChangesAsync(cancellationToken);
        return module;
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
                RenderMode = "generic",
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
            screen.RenderMode = "generic";
        }

        await _db.SaveChangesAsync(cancellationToken);

        var catalogCodes = CrmLeadsScreenCatalog.Sections.Select(x => x.Code).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var now = _clock.UtcNow;

        // Soft-delete legacy / unknown sections (e.g. old "main").
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

    private async Task<bool> EnsureOpportunityScreenAsync(Guid tenantId, Guid moduleId, CancellationToken cancellationToken)
    {
        var exists = await _db.ScreenDefinitions
            .IgnoreQueryFilters()
            .AnyAsync(
                x => x.TenantId == tenantId && x.Code == CrmOpportunitiesScreenCode && !x.IsDeleted,
                cancellationToken);

        if (exists)
        {
            return false;
        }

        var softDeleted = await _db.ScreenDefinitions
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x => x.TenantId == tenantId && x.Code == CrmOpportunitiesScreenCode,
                cancellationToken);

        if (softDeleted is not null)
        {
            softDeleted.IsDeleted = false;
            softDeleted.DeletedAt = null;
            softDeleted.DeletedBy = null;
            softDeleted.ModuleDefinitionId = moduleId;
            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }

        var screen = new ScreenDefinition
        {
            ModuleDefinitionId = moduleId,
            Code = CrmOpportunitiesScreenCode,
            Name = "CRM Opportunities",
            Route = "/crm/opportunities",
            RenderMode = "generic",
            EntityName = CrmOpportunitiesScreenCode,
            ApiBasePath = "/api/v1/crm/opportunities",
            WorkflowEnabled = false,
            PrintEnabled = false,
            AiEnabled = true
        };
        screen.SetTenantId(tenantId);

        var section = new SectionDefinition
        {
            Code = "main",
            Name = "Details",
            Description = "Opportunity details.",
            DisplayOrder = 1
        };
        section.SetTenantId(tenantId);

        foreach (var f in OpportunityFields)
        {
            var field = new FieldDefinition
            {
                FieldKey = f.Key,
                Label = f.Label,
                DataType = f.DataType,
                ControlType = f.ControlType,
                DisplayOrder = f.Order,
                IsRequired = f.Required,
                IsReadOnly = f.ReadOnly,
                IsVisible = true,
                Width = 3
            };
            field.SetTenantId(tenantId);
            section.Fields.Add(field);
        }

        screen.Sections.Add(section);
        _db.ScreenDefinitions.Add(screen);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static readonly IReadOnlyList<(string Key, string Label, string DataType, string ControlType, int Order, bool Required, bool ReadOnly)> OpportunityFields =
    [
        ("opportunityNumber", "Opportunity Number", "text", "input", 1, false, true),
        ("name", "Name", "text", "input", 2, true, false),
        ("stage", "Stage", "text", "input", 3, false, false),
        ("opportunityValue", "Opportunity Value", "number", "number", 4, false, false),
        ("status", "Status", "text", "input", 5, false, false),
        ("notes", "Notes", "text", "textarea", 6, false, false),
    ];
}
