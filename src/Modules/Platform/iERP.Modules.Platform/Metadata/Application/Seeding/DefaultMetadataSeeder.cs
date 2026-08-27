using iERP.Application.Abstractions.Seeding;
using iERP.Modules.Platform.Metadata.Domain;
using iERP.Modules.Platform.Metadata.Infrastructure;
using iERP.Modules.Platform.Tenancy.Infrastructure;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace iERP.Modules.Platform.Metadata.Application.Seeding;

/// <summary>
/// Seeds predefined Hybrid module/screen/section/field metadata for every tenant.
/// Runs on startup (local + Railway) so GET /api/v1/metadata/modules is never empty for existing tenants.
/// </summary>
public sealed class DefaultMetadataSeeder : IDataSeeder
{
    public const string CrmModuleCode = "crm";
    public const string CrmLeadsScreenCode = "crm-leads";
    public const string CrmOpportunitiesScreenCode = "crm-opportunities";

    private readonly MetadataDbContext _db;
    private readonly PlatformDbContext _platformDb;
    private readonly ITenantContext _tenantContext;
    private readonly ILogger<DefaultMetadataSeeder> _logger;

    public DefaultMetadataSeeder(
        MetadataDbContext db,
        PlatformDbContext platformDb,
        ITenantContext tenantContext,
        ILogger<DefaultMetadataSeeder> logger)
    {
        _db = db;
        _platformDb = platformDb;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var tenantIds = await _platformDb.Tenants
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        // Fallback: AuthSeed may have set tenant in this same startup scope before tenants query,
        // or a single-tenant deploy with tenants loaded differently.
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
        var leadsSeeded = await EnsureScreenAsync(
            tenantId,
            module.Id,
            CrmLeadsScreenCode,
            name: "CRM Leads",
            route: "/crm/leads",
            apiBasePath: "/api/v1/crm/leads",
            LeadFields,
            cancellationToken);

        var oppsSeeded = await EnsureScreenAsync(
            tenantId,
            module.Id,
            CrmOpportunitiesScreenCode,
            name: "CRM Opportunities",
            route: "/crm/opportunities",
            apiBasePath: "/api/v1/crm/opportunities",
            OpportunityFields,
            cancellationToken);

        if (leadsSeeded || oppsSeeded)
        {
            _logger.LogInformation(
                "Seeded predefined CRM metadata for tenant {TenantId} (leads={Leads}, opportunities={Opps})",
                tenantId,
                leadsSeeded,
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

        // Revive soft-deleted module with same code if present (unique index).
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

    private async Task<bool> EnsureScreenAsync(
        Guid tenantId,
        Guid moduleId,
        string screenCode,
        string name,
        string route,
        string apiBasePath,
        IReadOnlyList<SeedField> fields,
        CancellationToken cancellationToken)
    {
        var exists = await _db.ScreenDefinitions
            .IgnoreQueryFilters()
            .AnyAsync(
                x => x.TenantId == tenantId && x.Code == screenCode && !x.IsDeleted,
                cancellationToken);

        if (exists)
        {
            return false;
        }

        var softDeleted = await _db.ScreenDefinitions
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x => x.TenantId == tenantId && x.Code == screenCode,
                cancellationToken);

        if (softDeleted is not null)
        {
            softDeleted.IsDeleted = false;
            softDeleted.DeletedAt = null;
            softDeleted.DeletedBy = null;
            softDeleted.Name = name;
            softDeleted.Route = route;
            softDeleted.ApiBasePath = apiBasePath;
            softDeleted.ModuleDefinitionId = moduleId;
            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }

        var screen = new ScreenDefinition
        {
            ModuleDefinitionId = moduleId,
            Code = screenCode,
            Name = name,
            Route = route,
            RenderMode = "generic",
            EntityName = screenCode,
            ApiBasePath = apiBasePath,
            WorkflowEnabled = false,
            PrintEnabled = false,
            AiEnabled = true
        };
        screen.SetTenantId(tenantId);

        var section = new SectionDefinition
        {
            Code = "main",
            Name = "Details",
            DisplayOrder = 1
        };
        section.SetTenantId(tenantId);

        foreach (var f in fields)
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

    private static readonly IReadOnlyList<SeedField> LeadFields =
    [
        new("leadNumber", "Lead Number", "text", "input", 1, Required: false, ReadOnly: true),
        new("companyName", "Company Name", "text", "input", 2, Required: true, ReadOnly: false),
        new("contactPerson", "Contact Person", "text", "input", 3, Required: false, ReadOnly: false),
        new("phone", "Phone", "text", "input", 4, Required: true, ReadOnly: false),
        new("email", "Email", "text", "input", 5, Required: true, ReadOnly: false),
        new("status", "Status", "text", "input", 6, Required: false, ReadOnly: false),
        new("notes", "Notes", "text", "textarea", 7, Required: false, ReadOnly: false),
    ];

    private static readonly IReadOnlyList<SeedField> OpportunityFields =
    [
        new("opportunityNumber", "Opportunity Number", "text", "input", 1, Required: false, ReadOnly: true),
        new("name", "Name", "text", "input", 2, Required: true, ReadOnly: false),
        new("stage", "Stage", "text", "input", 3, Required: false, ReadOnly: false),
        new("opportunityValue", "Opportunity Value", "number", "number", 4, Required: false, ReadOnly: false),
        new("status", "Status", "text", "input", 5, Required: false, ReadOnly: false),
        new("notes", "Notes", "text", "textarea", 6, Required: false, ReadOnly: false),
    ];

    private sealed record SeedField(
        string Key,
        string Label,
        string DataType,
        string ControlType,
        int Order,
        bool Required,
        bool ReadOnly);
}
