using FluentValidation;
using iERP.Modules.CRM.Application.Opportunities.Commands;
using iERP.Modules.CRM.Application.Opportunities.Dtos;

namespace iERP.Modules.CRM.Application.Opportunities.Validators;

public sealed class ConvertLeadToOpportunityCommandValidator : AbstractValidator<ConvertLeadToOpportunityCommand>
{
    public ConvertLeadToOpportunityCommandValidator()
    {
        RuleFor(x => x.LeadId).NotEmpty();
        RuleFor(x => x.Request).SetValidator(new ConvertLeadToOpportunityRequestValidator());
    }
}

public sealed class UpdateOpportunityCommandValidator : AbstractValidator<UpdateOpportunityCommand>
{
    public UpdateOpportunityCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Request).SetValidator(new UpdateOpportunityRequestValidator());
    }
}

public sealed class CreateOpportunityFollowUpCommandValidator : AbstractValidator<CreateOpportunityFollowUpCommand>
{
    public CreateOpportunityFollowUpCommandValidator()
    {
        RuleFor(x => x.OpportunityId).NotEmpty();
        RuleFor(x => x.Request).SetValidator(new CreateOpportunityFollowUpRequestValidator());
    }
}

public sealed class UpdateOpportunityFollowUpCommandValidator : AbstractValidator<UpdateOpportunityFollowUpCommand>
{
    public UpdateOpportunityFollowUpCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Request).SetValidator(new UpdateOpportunityFollowUpRequestValidator());
    }
}
