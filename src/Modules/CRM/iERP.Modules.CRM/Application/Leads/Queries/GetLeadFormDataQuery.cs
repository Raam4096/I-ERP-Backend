using System.Globalization;
using iERP.Application.Abstractions.Metadata;
using iERP.Modules.CRM.Application.Leads.Dtos;
using iERP.Modules.CRM.Infrastructure;
using iERP.SharedKernel.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.CRM.Application.Leads.Queries;

/// <summary>
/// Section-wise lead form payload for metadata-driven UI (schema + values).
/// Field keys are snake_case to match CrmLeadsScreenCatalog / UI example JSON.
/// </summary>
public sealed class LeadFormDataDto
{
    public Guid Id { get; init; }
    public string ScreenCode { get; init; } = CrmLeadsScreenCatalog.ScreenCode;
    public string LeadNumber { get; init; } = string.Empty;
    public IReadOnlyList<LeadFormSectionDto> Sections { get; init; } = [];
    /// <summary>
    /// UI bag: section code → snake_case field values (same shape as CrmLeadsExamplePayload).
    /// </summary>
    public IReadOnlyDictionary<string, IReadOnlyDictionary<string, object?>> ValuesBySection { get; init; }
        = new Dictionary<string, IReadOnlyDictionary<string, object?>>();
}

public sealed class LeadFormSectionDto
{
    public string Code { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public IReadOnlyList<LeadFormFieldDto> Fields { get; init; } = [];
}

public sealed class LeadFormFieldDto
{
    public string FieldKey { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public string DataType { get; init; } = "text";
    public string ControlType { get; init; } = "input";
    public bool Required { get; init; }
    public bool ReadOnly { get; init; }
    public int DisplayOrder { get; init; }
    public object? Value { get; init; }
}

public sealed record GetLeadFormDataQuery(Guid Id) : IRequest<LeadFormDataDto>;

public sealed class GetLeadFormDataQueryHandler : IRequestHandler<GetLeadFormDataQuery, LeadFormDataDto>
{
    private readonly CrmDbContext _db;

    public GetLeadFormDataQueryHandler(CrmDbContext db)
    {
        _db = db;
    }

    public async Task<LeadFormDataDto> Handle(GetLeadFormDataQuery query, CancellationToken cancellationToken)
    {
        var lead = await _db.Leads
            .AsNoTracking()
            .Include(x => x.FollowUps)
                .ThenInclude(x => x.Attachments)
            .FirstOrDefaultAsync(x => x.Id == query.Id, cancellationToken)
            ?? throw new NotFoundException($"Lead '{query.Id}' was not found.");

        var latestFollowUp = lead.FollowUps
            .OrderByDescending(x => x.FollowUpDate)
            .ThenByDescending(x => x.CreatedAt)
            .FirstOrDefault();

        var valueMap = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
        {
            ["company_name"] = lead.CompanyName,
            ["contact_person"] = lead.ContactPerson,
            ["phone_number"] = lead.Phone,
            ["email"] = lead.Email,
            ["industry"] = lead.Industry,
            ["project_type"] = lead.ProjectType,
            ["lead_source"] = lead.LeadSource,
            ["status"] = lead.Status,
            ["assigned_to"] = lead.AssignedToUserId?.ToString(),
            ["website"] = lead.Website,
            ["company_size"] = lead.CompanySize,
            ["annual_revenue"] = lead.AnnualRevenue?.ToString(CultureInfo.InvariantCulture),
            ["address"] = lead.Address,
            ["subsidiary"] = lead.Subsidiary,
            ["project_description"] = lead.ProjectDescription,
            ["notes"] = lead.Notes,
            ["follow_up_date"] = FormatDisplayDate(latestFollowUp?.FollowUpDate),
            ["new_follow_up_date"] = FormatDisplayDate(latestFollowUp?.NextFollowUpDate),
            ["follow_up_status"] = latestFollowUp?.Status,
            ["follow_up_type"] = latestFollowUp?.ActivityType,
            ["follow_up_file"] = latestFollowUp?.Attachments.OrderByDescending(a => a.CreatedAt).FirstOrDefault()?.FileName,
            ["follow_up_notes"] = latestFollowUp?.Remarks
        };

        var sections = new List<LeadFormSectionDto>();
        var valuesBySection = new Dictionary<string, IReadOnlyDictionary<string, object?>>(StringComparer.OrdinalIgnoreCase);

        foreach (var sectionSpec in CrmLeadsScreenCatalog.Sections.OrderBy(x => x.Order))
        {
            var sectionValues = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            var fields = new List<LeadFormFieldDto>();

            foreach (var fieldSpec in sectionSpec.Fields.OrderBy(x => x.Order))
            {
                valueMap.TryGetValue(fieldSpec.FieldKey, out var value);
                sectionValues[fieldSpec.FieldKey] = value;

                fields.Add(new LeadFormFieldDto
                {
                    FieldKey = fieldSpec.FieldKey,
                    Label = fieldSpec.Label,
                    DataType = fieldSpec.DataType,
                    ControlType = fieldSpec.ControlType,
                    Required = fieldSpec.Required,
                    ReadOnly = fieldSpec.ReadOnly,
                    DisplayOrder = fieldSpec.Order,
                    Value = value
                });
            }

            sections.Add(new LeadFormSectionDto
            {
                Code = sectionSpec.Code,
                Title = sectionSpec.Name,
                Description = sectionSpec.Description,
                Fields = fields
            });
            valuesBySection[sectionSpec.Code] = sectionValues;
        }

        return new LeadFormDataDto
        {
            Id = lead.Id,
            LeadNumber = lead.LeadNumber,
            Sections = sections,
            ValuesBySection = valuesBySection
        };
    }

    private static string? FormatDisplayDate(DateTimeOffset? value) =>
        value?.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
}

/// <summary>
/// Static example payload for UI Leads layout (matches CrmLeadsExamplePayload).
/// </summary>
public sealed class LeadExampleFormDto
{
    public string ScreenCode { get; init; } = CrmLeadsScreenCatalog.ScreenCode;
    public string ScreenName { get; init; } = CrmLeadsScreenCatalog.ScreenName;
    public IReadOnlyList<LeadFormSectionDto> Sections { get; init; } = [];
    public IReadOnlyDictionary<string, IReadOnlyDictionary<string, object?>> ValuesBySection { get; init; }
        = CrmLeadsExamplePayload.ValuesBySection;
}

public sealed record GetLeadExampleFormQuery : IRequest<LeadExampleFormDto>;

public sealed class GetLeadExampleFormQueryHandler : IRequestHandler<GetLeadExampleFormQuery, LeadExampleFormDto>
{
    public Task<LeadExampleFormDto> Handle(GetLeadExampleFormQuery request, CancellationToken cancellationToken)
    {
        var sections = CrmLeadsScreenCatalog.Sections
            .OrderBy(x => x.Order)
            .Select(section =>
            {
                CrmLeadsExamplePayload.ValuesBySection.TryGetValue(section.Code, out var sectionValues);
                sectionValues ??= new Dictionary<string, object?>();

                return new LeadFormSectionDto
                {
                    Code = section.Code,
                    Title = section.Name,
                    Description = section.Description,
                    Fields = section.Fields
                        .OrderBy(f => f.Order)
                        .Select(f =>
                        {
                            sectionValues.TryGetValue(f.FieldKey, out var value);
                            return new LeadFormFieldDto
                            {
                                FieldKey = f.FieldKey,
                                Label = f.Label,
                                DataType = f.DataType,
                                ControlType = f.ControlType,
                                Required = f.Required,
                                ReadOnly = f.ReadOnly,
                                DisplayOrder = f.Order,
                                Value = value
                            };
                        })
                        .ToList()
                };
            })
            .ToList();

        return Task.FromResult(new LeadExampleFormDto
        {
            Sections = sections,
            ValuesBySection = CrmLeadsExamplePayload.ValuesBySection
        });
    }
}
