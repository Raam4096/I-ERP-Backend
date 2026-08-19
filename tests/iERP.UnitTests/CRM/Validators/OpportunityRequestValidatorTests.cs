using iERP.Modules.CRM.Application.Opportunities.Dtos;
using iERP.Modules.CRM.Application.Opportunities.Validators;
using iERP.UnitTests.Common;

namespace iERP.UnitTests.CRM.Validators;

public sealed class OpportunityRequestValidatorTests
{
    private readonly ConvertLeadToOpportunityRequestValidator _convert = new();

    [Theory]
    [MemberData(nameof(InvalidConvertCases))]
    public async Task Convert_rejects_invalid_payload(
        ConvertLeadToOpportunityRequest request,
        string expectedPropertyOrMessageHint)
    {
        var result = await _convert.ValidateAsync(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(
            e => e.PropertyName.Contains(expectedPropertyOrMessageHint, StringComparison.OrdinalIgnoreCase)
                 || e.ErrorMessage.Contains(expectedPropertyOrMessageHint, StringComparison.OrdinalIgnoreCase));
    }

    public static TheoryData<ConvertLeadToOpportunityRequest, string> InvalidConvertCases => new()
    {
        { CrmRequestFactory.ValidConvert(opportunityValue: -1), "OpportunityValue" },
        { CrmRequestFactory.ValidConvert(probability: -1), "Probability" },
        { CrmRequestFactory.ValidConvert(probability: 101), "Probability" },
        { CrmRequestFactory.ValidConvert(status: "Nope"), "status" },
        { CrmRequestFactory.ValidConvert(status: "Won", closedReason: null), "Closed reason" },
        { CrmRequestFactory.ValidConvert(status: "Lost", closedReason: "  "), "Closed reason" },
    };

    [Fact]
    public async Task Convert_accepts_valid_payload()
    {
        await _convert.ShouldBeValidAsync(CrmRequestFactory.ValidConvert());
    }
}
