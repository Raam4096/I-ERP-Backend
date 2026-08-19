using iERP.Modules.CRM.Application.Leads.Validators;
using iERP.Modules.CRM.Application.Leads.Dtos;
using iERP.UnitTests.Common;

namespace iERP.UnitTests.CRM.Validators;

public sealed class LeadRequestValidatorTests
{
    private readonly CreateLeadRequestValidator _create = new();
    private readonly CreateFollowUpRequestValidator _followUp = new();

    [Theory]
    [MemberData(nameof(MissingLeadCases))]
    public async Task CreateLead_rejects_missing_or_invalid_core_fields(
        CreateLeadRequest request,
        string expectedProperty)
    {
        await _create.ShouldHaveValidationErrorForAsync(request, expectedProperty);
    }

    public static TheoryData<CreateLeadRequest, string> MissingLeadCases => new()
    {
        { CrmRequestFactory.ValidLead(companyName: ""), "CompanyName" },
        { CrmRequestFactory.ValidLead(phone: ""), "Phone" },
        { CrmRequestFactory.ValidLead(email: ""), "Email" },
        { CrmRequestFactory.ValidLead(email: "not-an-email"), "Email" },
        { CrmRequestFactory.ValidLead(website: "ftp://bad"), "Website" },
    };

    [Fact]
    public async Task CreateLead_accepts_valid_request()
    {
        await _create.ShouldBeValidAsync(CrmRequestFactory.ValidLead(website: "https://acme.test"));
    }

    [Fact]
    public async Task FollowUp_rejects_next_date_before_follow_up_date()
    {
        var request = CrmRequestFactory.ValidFollowUp(
            followUpDate: new DateTimeOffset(2026, 8, 20, 0, 0, 0, TimeSpan.Zero),
            nextFollowUpDate: new DateTimeOffset(2026, 8, 19, 0, 0, 0, TimeSpan.Zero));

        await _followUp.ShouldHaveValidationErrorForAsync(request, "on or after follow-up date");
    }

    [Fact]
    public async Task FollowUp_rejects_missing_activity_type()
    {
        var request = CrmRequestFactory.ValidFollowUp(activityType: "");

        await _followUp.ShouldHaveValidationErrorForAsync(request, "ActivityType");
    }
}
