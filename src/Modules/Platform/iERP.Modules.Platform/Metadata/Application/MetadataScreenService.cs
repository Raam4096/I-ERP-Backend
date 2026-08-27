using iERP.Modules.Platform.Metadata.Application.Dtos;
using iERP.Modules.Platform.Metadata.Application.Layout;
using iERP.Modules.Platform.Metadata.Infrastructure;
using iERP.SharedKernel.Exceptions;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.Platform.Metadata.Application;

public interface IMetadataScreenService
{
    Task<GenericPageDto> GetScreenAsync(string screenCode, CancellationToken cancellationToken);
}

public sealed class MetadataScreenService : IMetadataScreenService
{
    private readonly MetadataDbContext _db;
    private readonly ITenantContext _tenantContext;
    private readonly IUserFieldPreferenceService _preferenceService;

    public MetadataScreenService(
        MetadataDbContext db,
        ITenantContext tenantContext,
        IUserFieldPreferenceService preferenceService)
    {
        _db = db;
        _tenantContext = tenantContext;
        _preferenceService = preferenceService;
    }

    public async Task<GenericPageDto> GetScreenAsync(string screenCode, CancellationToken cancellationToken)
    {
        if (!_tenantContext.HasTenant)
        {
            throw new ForbiddenException("Tenant context is required.", ErrorCodes.TenantNotFound);
        }

        var code = screenCode.Trim();
        var screen = await _db.ScreenDefinitions
            .AsNoTracking()
            .Include(x => x.Sections)
            .ThenInclude(s => s.Fields)
            .FirstOrDefaultAsync(x => x.Code == code, cancellationToken)
            ?? throw new NotFoundException($"Screen '{code}' was not found.");

        var entityKey = string.IsNullOrWhiteSpace(screen.EntityName) ? screen.Code : screen.EntityName;
        var customFields = await _db.CustomFieldDefinitions
            .AsNoTracking()
            .Where(x => x.IsActive && (x.EntityName == screen.Code || x.EntityName == entityKey))
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync(cancellationToken);

        var preferences = await _preferenceService.GetPreferencesAsync(code, cancellationToken);

        var sections = screen.Sections
            .OrderBy(s => s.DisplayOrder)
            .Select(section =>
            {
                var coreFields = section.Fields
                    .OrderBy(f => f.DisplayOrder)
                    .Select(f => new GenericPageFieldDto
                    {
                        FieldKey = f.FieldKey,
                        Label = f.Label,
                        DataType = f.DataType,
                        ControlType = f.ControlType,
                        Required = f.IsRequired,
                        ReadOnly = f.IsReadOnly,
                        Visible = f.IsVisible,
                        Width = f.Width ?? 3,
                        DisplayOrder = f.DisplayOrder,
                        IsCustom = false
                    });

                var merged = coreFields.ToList();
                if (section.DisplayOrder == screen.Sections.Min(x => x.DisplayOrder))
                {
                    foreach (var custom in customFields)
                    {
                        merged.Add(new GenericPageFieldDto
                        {
                            FieldKey = custom.FieldKey,
                            Label = custom.Label,
                            DataType = custom.DataType,
                            ControlType = MapControl(custom.DataType),
                            Required = custom.IsRequired,
                            ReadOnly = false,
                            Visible = true,
                            Width = 3,
                            DisplayOrder = custom.DisplayOrder,
                            IsCustom = true
                        });
                    }
                }

                return new GenericPageSectionDto
                {
                    Code = section.Code,
                    Title = section.Name,
                    Type = "header",
                    Fields = ApplyPreferences(merged, preferences)
                };
            })
            .ToList();

        return new GenericPageDto
        {
            Screen = new GenericPageScreenDto
            {
                Code = screen.Code,
                Name = screen.Name,
                Route = screen.Route,
                RenderMode = string.IsNullOrWhiteSpace(screen.RenderMode) ? "generic" : screen.RenderMode,
                EntityName = screen.EntityName,
                ApiBasePath = screen.ApiBasePath,
                WorkflowEnabled = screen.WorkflowEnabled,
                PrintEnabled = screen.PrintEnabled,
                AiEnabled = screen.AiEnabled
            },
            Layout = new GenericPageLayoutDto(),
            Sections = sections,
            Actions =
            [
                new GenericPageActionDto
                {
                    ActionKey = "save",
                    Label = "Save",
                    ActionType = "api",
                    Endpoint = screen.ApiBasePath
                }
            ]
        };
    }

    public static IReadOnlyList<GenericPageFieldDto> ApplyPreferences(
        IEnumerable<GenericPageFieldDto> fields,
        IReadOnlyDictionary<string, FieldPreferenceValue> preferences)
    {
        var fieldList = fields.ToList();
        var states = FieldLayoutPreferenceApplier.ApplyAll(
            fieldList.Select(f => new FieldLayoutState(f.FieldKey, f.Required, f.Visible, f.DisplayOrder)),
            preferences);

        var byKey = fieldList.ToDictionary(f => f.FieldKey, StringComparer.OrdinalIgnoreCase);
        return states
            .Where(s => byKey.ContainsKey(s.FieldKey))
            .Select(s =>
            {
                var source = byKey[s.FieldKey];
                return new GenericPageFieldDto
                {
                    FieldKey = source.FieldKey,
                    Label = source.Label,
                    DataType = source.DataType,
                    ControlType = source.ControlType,
                    Required = source.Required,
                    ReadOnly = source.ReadOnly,
                    Visible = s.Visible,
                    Width = source.Width,
                    DisplayOrder = s.DisplayOrder,
                    IsCustom = source.IsCustom
                };
            })
            .ToList();
    }

    private static string MapControl(string dataType) =>
        dataType.ToLowerInvariant() switch
        {
            "number" or "decimal" or "int" => "number",
            "date" or "datetime" => "datepicker",
            "boolean" or "bool" => "checkbox",
            _ => "input"
        };
}
