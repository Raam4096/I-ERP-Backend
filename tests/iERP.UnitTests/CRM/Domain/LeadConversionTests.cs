using iERP.Modules.CRM.Domain;
using iERP.SharedKernel.Exceptions;

namespace iERP.UnitTests.CRM.Domain;

public sealed class LeadConversionTests
{
    [Fact]
    public void MarkConverted_succeeds_once()
    {
        var lead = CrmDomainFactory.CreateLead(status: LeadStatuses.Qualified);
        var opportunityId = Guid.NewGuid();

        lead.MarkConverted(opportunityId, DateTimeOffset.UtcNow);

        lead.Status.Should().Be(LeadStatuses.Converted);
        lead.ConvertedOpportunityId.Should().Be(opportunityId);
        lead.ConvertedAt.Should().NotBeNull();
        lead.ConvertedAt!.Value.Offset.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void MarkConverted_twice_throws_conflict_message()
    {
        var lead = CrmDomainFactory.CreateLead(status: LeadStatuses.Qualified);
        lead.MarkConverted(Guid.NewGuid(), DateTimeOffset.UtcNow);

        var act = () => lead.MarkConverted(Guid.NewGuid(), DateTimeOffset.UtcNow);

        act.Should().Throw<BusinessRuleException>()
            .Where(e => e.ErrorCode == ErrorCodes.BusinessRuleViolation)
            .WithMessage("*already been converted*");
    }

    [Fact]
    public void MarkConverted_unqualified_throws()
    {
        var lead = CrmDomainFactory.CreateLead(status: LeadStatuses.Unqualified);

        var act = () => lead.MarkConverted(Guid.NewGuid(), DateTimeOffset.UtcNow);

        act.Should().Throw<BusinessRuleException>()
            .WithMessage("*Unqualified*");
    }

    [Fact]
    public void MarkConverted_normalizes_ist_to_utc()
    {
        var lead = CrmDomainFactory.CreateLead(status: LeadStatuses.Qualified);
        var ist = new DateTimeOffset(2026, 8, 20, 12, 0, 0, TimeSpan.FromHours(5.5));

        lead.MarkConverted(Guid.NewGuid(), ist);

        lead.ConvertedAt.Should().Be(new DateTimeOffset(2026, 8, 20, 6, 30, 0, TimeSpan.Zero));
    }
}
