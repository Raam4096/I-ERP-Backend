using iERP.Modules.CRM.Domain;
using iERP.SharedKernel.Exceptions;

namespace iERP.UnitTests.CRM.Domain;

public sealed class OpportunityLifecycleTests
{
    [Fact]
    public void Discard_then_restore_round_trips_status()
    {
        var lead = CrmDomainFactory.CreateLead();
        var opportunity = CrmDomainFactory.CreateOpportunityFrom(lead);
        opportunity.Update(2000, 60, OpportunityStatuses.InProgress, null, null, null, "USD", null, null, null);

        opportunity.Discard();
        opportunity.Status.Should().Be(OpportunityStatuses.Discarded);

        opportunity.Restore();
        opportunity.Status.Should().Be(OpportunityStatuses.InProgress);
    }

    [Fact]
    public void Discard_twice_throws()
    {
        var opportunity = CrmDomainFactory.CreateOpportunityFrom(CrmDomainFactory.CreateLead());
        opportunity.Discard();

        var act = () => opportunity.Discard();

        act.Should().Throw<BusinessRuleException>()
            .WithMessage("*already discarded*");
    }

    [Fact]
    public void Update_while_discarded_throws()
    {
        var opportunity = CrmDomainFactory.CreateOpportunityFrom(CrmDomainFactory.CreateLead());
        opportunity.Discard();

        var act = () => opportunity.Update(1, 1, OpportunityStatuses.New, null, null, null, "USD", null, null, null);

        act.Should().Throw<BusinessRuleException>()
            .WithMessage("*Restore first*");
    }

    [Fact]
    public void Restore_when_not_discarded_throws()
    {
        var opportunity = CrmDomainFactory.CreateOpportunityFrom(CrmDomainFactory.CreateLead());

        var act = () => opportunity.Restore();

        act.Should().Throw<BusinessRuleException>()
            .WithMessage("*Only discarded*");
    }
}
