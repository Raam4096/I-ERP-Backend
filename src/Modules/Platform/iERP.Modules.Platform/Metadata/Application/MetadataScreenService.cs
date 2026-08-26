using iERP.Modules.Platform.Metadata.Application.Dtos;
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

    public MetadataScreenService(MetadataDbContext db, ITenantContext tenantContext)
    {
        _db = db;
        _tenantContext = tenantContext;
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

                // Append custom fields to the first section (ProcessFlow auto-render rule).
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
                    Fields = merged.OrderBy(f => f.DisplayOrder).ToList()
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

    private static string MapControl(string dataType) =>
        dataType.ToLowerInvariant() switch
        {
            "number" or "decimal" or "int" => "number",
            "date" or "datetime" => "datepicker",
            "boolean" or "bool" => "checkbox",
            _ => "input"
        };
}
