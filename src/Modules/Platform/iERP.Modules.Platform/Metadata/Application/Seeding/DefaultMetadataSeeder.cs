using iERP.Application.Abstractions.Seeding;
using iERP.Modules.Platform.Metadata.Domain;
using iERP.Modules.Platform.Metadata.Infrastructure;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace iERP.Modules.Platform.Metadata.Application.Seeding;

/// <summary>
/// Seeds CRM Hybrid screen metadata (leads) for GenericPage — ProcessFlow v4 aligned.
/// </summary>
public sealed class DefaultMetadataSeeder : IDataSeeder
{
    public const string CrmLeadsScreenCode = "crm-leads";

    private readonly MetadataDbContext _db;
    private readonly ITenantContext _tenantContext;
    private readonly ILogger<DefaultMetadataSeeder> _logger;

    public DefaultMetadataSeeder(
        MetadataDbContext db,
        ITenantContext tenantContext,
        ILogger<DefaultMetadataSeeder> logger)
    {
        _db = db;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!_tenantContext.HasTenant)
        {
            return;
        }

        var tenantId = _tenantContext.TenantId!.Value;

        var module = await _db.ModuleDefinitions
            .FirstOrDefaultAsync(x => x.Code == "crm", cancellationToken);

        if (module is null)
        {
            module = new ModuleDefinition
            {
                Code = "crm",
                Name = "CRM",
                Description = "Customer relationship management",
                IsActive = true
            };
            module.SetTenantId(tenantId);
            _db.ModuleDefinitions.Add(module);
            await _db.SaveChangesAsync(cancellationToken);
        }

        var screen = await _db.ScreenDefinitions
            .Include(x => x.Sections)
            .ThenInclude(s => s.Fields)
            .FirstOrDefaultAsync(x => x.Code == CrmLeadsScreenCode, cancellationToken);

        if (screen is not null)
        {
            return;
        }

        screen = new ScreenDefinition
        {
            ModuleDefinitionId = module.Id,
            Code = CrmLeadsScreenCode,
            Name = "CRM Leads",
            Route = "/crm/leads",
            RenderMode = "generic",
            EntityName = CrmLeadsScreenCode,
            ApiBasePath = "/api/v1/crm/leads",
            WorkflowEnabled = false,
            PrintEnabled = false,
            AiEnabled = true
        };
        screen.SetTenantId(tenantId);

        var section = new SectionDefinition
        {
            Code = "main",
            Name = "Lead Details",
            DisplayOrder = 1
        };
        section.SetTenantId(tenantId);

        var fields = new (string Key, string Label, string DataType, string Control, int Order, bool Required)[]
        {
            ("leadNumber", "Lead Number", "text", "input", 1, false),
            ("companyName", "Company Name", "text", "input", 2, true),
            ("contactPerson", "Contact Person", "text", "input", 3, false),
            ("phone", "Phone", "text", "input", 4, true),
            ("email", "Email", "text", "input", 5, true),
            ("status", "Status", "text", "input", 6, false),
            ("notes", "Notes", "text", "textarea", 7, false),
        };

        foreach (var f in fields)
        {
            var field = new FieldDefinition
            {
                FieldKey = f.Key,
                Label = f.Label,
                DataType = f.DataType,
                ControlType = f.Control,
                DisplayOrder = f.Order,
                IsRequired = f.Required,
                IsReadOnly = f.Key == "leadNumber",
                IsVisible = true,
                Width = 3
            };
            field.SetTenantId(tenantId);
            section.Fields.Add(field);
        }

        screen.Sections.Add(section);
        _db.ScreenDefinitions.Add(screen);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Seeded metadata screen {ScreenCode} for tenant {TenantId}",
            CrmLeadsScreenCode,
            tenantId);
    }
}
