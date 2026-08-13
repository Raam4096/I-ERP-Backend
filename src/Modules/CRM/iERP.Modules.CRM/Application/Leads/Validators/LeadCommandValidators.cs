using FluentValidation;
using iERP.Modules.CRM.Application.Leads.Commands;
using iERP.Modules.CRM.Application.Leads.Dtos;

namespace iERP.Modules.CRM.Application.Leads.Validators;

public sealed class CreateLeadCommandValidator : AbstractValidator<CreateLeadCommand>
{
    public CreateLeadCommandValidator()
    {
        RuleFor(x => x.Request).SetValidator(new CreateLeadRequestValidator());
    }
}

public sealed class UpdateLeadCommandValidator : AbstractValidator<UpdateLeadCommand>
{
    public UpdateLeadCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Request).SetValidator(new UpdateLeadRequestValidator());
    }
}

public sealed class CreateFollowUpCommandValidator : AbstractValidator<CreateFollowUpCommand>
{
    public CreateFollowUpCommandValidator()
    {
        RuleFor(x => x.LeadId).NotEmpty();
        RuleFor(x => x.Request).SetValidator(new CreateFollowUpRequestValidator());
    }
}

public sealed class UpdateFollowUpCommandValidator : AbstractValidator<UpdateFollowUpCommand>
{
    public UpdateFollowUpCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Request).SetValidator(new UpdateFollowUpRequestValidator());
    }
}
