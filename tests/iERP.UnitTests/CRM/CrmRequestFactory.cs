using iERP.Modules.CRM.Application.Leads.Dtos;
using iERP.Modules.CRM.Application.Opportunities.Dtos;

namespace iERP.UnitTests.CRM;

/// <summary>
/// Shared valid request builders — override only the field under test (DRY).
/// </summary>
internal static class CrmRequestFactory
{
    public static CreateLeadRequest ValidLead(
        string companyName = "Acme Marine",
        string phone = "+6591234567",
        string email = "sales@acme.test",
        string? website = null,
        FollowUpInputDto? followUp = null) =>
        new(
            CompanyName: companyName,
            ContactPerson: "Jane",
            Phone: phone,
            Email: email,
            Industry: null,
            Address: null,
            AnnualRevenue: null,
            AssignedTo: null,
            CompanySize: null,
            LeadSource: null,
            ProjectDescription: null,
            ProjectType: null,
            Status: "Qualified",
            Subsidiary: null,
            SubsidiaryId: null,
            Website: website,
            Notes: null,
            FollowUp: followUp);

    public static CreateFollowUpRequest ValidFollowUp(
        string activityType = "Call",
        DateTimeOffset? followUpDate = null,
        DateTimeOffset? nextFollowUpDate = null) =>
        new(
            ActivityType: activityType,
            FollowUpDate: followUpDate ?? new DateTimeOffset(2026, 8, 20, 6, 30, 0, TimeSpan.Zero),
            NextFollowUpDate: nextFollowUpDate,
            Remarks: "ok",
            Status: "Open",
            Attachments: null);

    public static ConvertLeadToOpportunityRequest ValidConvert(
        decimal opportunityValue = 1000,
        int probability = 50,
        string? status = "New",
        string? closedReason = null,
        OpportunityFollowUpInputDto? followUp = null) =>
        new(
            OpportunityValue: opportunityValue,
            Probability: probability,
            Status: status,
            Computations: "ok",
            Notes: "ok",
            ClosedReason: closedReason,
            CurrencyCode: "USD",
            ExpectedCloseDate: null,
            OwnerUserId: null,
            FollowUp: followUp);
}
