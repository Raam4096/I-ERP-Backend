using System.Globalization;
using System.Text;
using iERP.Application.Abstractions.Metadata;
using iERP.Modules.CRM.Application.Leads.Dtos;
using iERP.Modules.CRM.Infrastructure;
using iERP.SharedKernel.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.CRM.Application.Leads.Queries;

/// <summary>
/// Section-wise lead form payload for metadata-driven UI (schema + values).
/// </summary>
public sealed class LeadFormDataDto
{
    public Guid Id { get; init; }
    public string ScreenCode { get; init; } = CrmLeadsScreenCatalog.ScreenCode;
    public string LeadNumber { get; init; } = string.Empty;
    public IReadOnlyList<LeadFormSectionDto> Sections { get; init; } = [];
    /// <summary>
    /// UI-friendly bag matching section codes → field values (camelCase keys matching CRM APIs).
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
            ["companyName"] = lead.CompanyName,
            ["contactPerson"] = lead.ContactPerson,
            ["phone"] = lead.Phone,
            ["email"] = lead.Email,
            ["industry"] = lead.Industry,
            ["projectType"] = lead.ProjectType,
            ["leadSource"] = lead.LeadSource,
            ["status"] = lead.Status,
            ["assignedTo"] = lead.AssignedToUserId,
            ["website"] = lead.Website,
            ["companySize"] = lead.CompanySize,
            ["annualRevenue"] = lead.AnnualRevenue,
            ["address"] = lead.Address,
            ["subsidiary"] = lead.Subsidiary,
            ["projectDescription"] = lead.ProjectDescription,
            ["notes"] = lead.Notes,
            ["followUpDate"] = latestFollowUp?.FollowUpDate,
            ["newFollowUpDate"] = latestFollowUp?.NextFollowUpDate,
            ["followUpStatus"] = latestFollowUp?.Status,
            ["followUpType"] = latestFollowUp?.ActivityType,
            ["followUpFile"] = latestFollowUp?.Attachments.OrderByDescending(a => a.CreatedAt).FirstOrDefault()?.FileName,
            ["followUpNotes"] = latestFollowUp?.Remarks
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
                // Also expose snake_case alias for UI samples that use company_name etc.
                sectionValues[ToSnakeCase(fieldSpec.FieldKey)] = value;

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

    private static string ToSnakeCase(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        var sb = new StringBuilder(value.Length + 4);
        for (var i = 0; i < value.Length; i++)
        {
            var c = value[i];
            if (char.IsUpper(c))
            {
                if (i > 0)
                {
                    sb.Append('_');
                }

                sb.Append(char.ToLower(c, CultureInfo.InvariantCulture));
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }
}
