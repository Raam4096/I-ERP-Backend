using iERP.SharedKernel.Exceptions;
using iERP.SharedKernel.Primitives;
using iERP.SharedKernel.Time;

namespace iERP.Modules.CRM.Domain;

/// <summary>
/// Lead aggregate root. Owns follow-ups; follow-ups own attachments.
/// </summary>
public sealed class Lead : AuditableEntity
{
    private readonly List<LeadFollowUp> _followUps = [];

    private Lead()
    {
    }

    public string LeadNumber { get; private set; } = string.Empty;
    public string CompanyName { get; private set; } = string.Empty;
    public string? ContactPerson { get; private set; }
    public string Phone { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? Industry { get; private set; }
    public string? Address { get; private set; }
    public decimal? AnnualRevenue { get; private set; }
    public Guid? AssignedToUserId { get; private set; }
    public string? CompanySize { get; private set; }
    public string? LeadSource { get; private set; }
    public string? ProjectDescription { get; private set; }
    public string? ProjectType { get; private set; }
    public string Status { get; private set; } = LeadStatuses.New;
    public string? Subsidiary { get; private set; }
    public Guid? SubsidiaryId { get; private set; }
    public string? Website { get; private set; }
    public string? Notes { get; private set; }

    // Retained for future Opportunity conversion without coupling now.
    public Guid? ConvertedCustomerId { get; private set; }
    public Guid? ConvertedContactId { get; private set; }
    public Guid? ConvertedOpportunityId { get; private set; }
    public DateTimeOffset? ConvertedAt { get; private set; }

    public IReadOnlyCollection<LeadFollowUp> FollowUps => _followUps.AsReadOnly();

    public static Lead Create(
        Guid tenantId,
        string leadNumber,
        string companyName,
        string phone,
        string email,
        string? contactPerson,
        string? industry,
        string? address,
        decimal? annualRevenue,
        Guid? assignedToUserId,
        string? companySize,
        string? leadSource,
        string? projectDescription,
        string? projectType,
        string? status,
        string? subsidiary,
        Guid? subsidiaryId,
        string? website,
        string? notes)
    {
        var lead = new Lead();
        lead.SetTenantId(tenantId);
        lead.LeadNumber = leadNumber.Trim();
        lead.ApplyDetails(
            companyName,
            phone,
            email,
            contactPerson,
            industry,
            address,
            annualRevenue,
            assignedToUserId,
            companySize,
            leadSource,
            projectDescription,
            projectType,
            string.IsNullOrWhiteSpace(status) ? LeadStatuses.New : status,
            subsidiary,
            subsidiaryId,
            website,
            notes);
        return lead;
    }

    public void Update(
        string companyName,
        string phone,
        string email,
        string? contactPerson,
        string? industry,
        string? address,
        decimal? annualRevenue,
        Guid? assignedToUserId,
        string? companySize,
        string? leadSource,
        string? projectDescription,
        string? projectType,
        string? status,
        string? subsidiary,
        Guid? subsidiaryId,
        string? website,
        string? notes)
    {
        ApplyDetails(
            companyName,
            phone,
            email,
            contactPerson,
            industry,
            address,
            annualRevenue,
            assignedToUserId,
            companySize,
            leadSource,
            projectDescription,
            projectType,
            status ?? Status,
            subsidiary,
            subsidiaryId,
            website,
            notes);
    }

    public LeadFollowUp AddFollowUp(
        string activityType,
        DateTimeOffset followUpDate,
        DateTimeOffset? nextFollowUpDate,
        string? remarks,
        string? status)
    {
        var followUp = LeadFollowUp.Create(
            TenantId,
            Id,
            activityType,
            followUpDate,
            nextFollowUpDate,
            remarks,
            status);
        _followUps.Add(followUp);
        return followUp;
    }

    public void MarkDeleted(Guid? deletedBy, DateTimeOffset deletedAt) => SoftDelete(deletedBy, deletedAt);

    public void MarkConverted(Guid opportunityId, DateTimeOffset convertedAt)
    {
        if (IsDeleted)
        {
            throw new BusinessRuleException(
                ErrorCodes.BusinessRuleViolation,
                "Deleted leads cannot be converted.");
        }

        if (ConvertedOpportunityId.HasValue ||
            Status.Equals(LeadStatuses.Converted, StringComparison.OrdinalIgnoreCase))
        {
            throw new BusinessRuleException(
                ErrorCodes.BusinessRuleViolation,
                "Lead has already been converted to an opportunity.");
        }

        if (Status.Equals(LeadStatuses.Unqualified, StringComparison.OrdinalIgnoreCase))
        {
            throw new BusinessRuleException(
                ErrorCodes.BusinessRuleViolation,
                "Unqualified leads cannot be converted.");
        }

        ConvertedOpportunityId = opportunityId;
        ConvertedAt = DateTimeOffsetUtc.Normalize(convertedAt);
        Status = LeadStatuses.Converted;
    }

    private void ApplyDetails(
        string companyName,
        string phone,
        string email,
        string? contactPerson,
        string? industry,
        string? address,
        decimal? annualRevenue,
        Guid? assignedToUserId,
        string? companySize,
        string? leadSource,
        string? projectDescription,
        string? projectType,
        string status,
        string? subsidiary,
        Guid? subsidiaryId,
        string? website,
        string? notes)
    {
        CompanyName = companyName.Trim();
        Phone = phone.Trim();
        Email = email.Trim().ToLowerInvariant();
        ContactPerson = Normalize(contactPerson);
        Industry = Normalize(industry);
        Address = Normalize(address);
        AnnualRevenue = annualRevenue;
        AssignedToUserId = assignedToUserId;
        CompanySize = Normalize(companySize);
        LeadSource = Normalize(leadSource);
        ProjectDescription = Normalize(projectDescription);
        ProjectType = Normalize(projectType);
        Status = status.Trim();
        Subsidiary = Normalize(subsidiary);
        SubsidiaryId = subsidiaryId;
        Website = Normalize(website);
        Notes = Normalize(notes);
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
