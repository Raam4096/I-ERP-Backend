using FluentValidation;
using iERP.Modules.CRM.Application.Opportunities.Dtos;
using iERP.Modules.CRM.Domain;

namespace iERP.Modules.CRM.Application.Opportunities.Validators;

public sealed class ConvertLeadToOpportunityRequestValidator : AbstractValidator<ConvertLeadToOpportunityRequest>
{
    public ConvertLeadToOpportunityRequestValidator()
    {
        RuleFor(x => x.OpportunityValue).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Probability).InclusiveBetween(0, 100);
        RuleFor(x => x.Status).MaximumLength(64)
            .Must(s => string.IsNullOrWhiteSpace(s) || OpportunityStatuses.All.Contains(s!))
            .WithMessage("Invalid opportunity status.");
        RuleFor(x => x.Computations).MaximumLength(4000);
        RuleFor(x => x.Notes).MaximumLength(4000);
        RuleFor(x => x.ClosedReason).MaximumLength(1024);
        RuleFor(x => x.CurrencyCode).MaximumLength(16);
        RuleFor(x => x)
            .Must(x =>
            {
                var status = string.IsNullOrWhiteSpace(x.Status) ? OpportunityStatuses.New : x.Status;
                return !OpportunityStatuses.IsClosed(status) || !string.IsNullOrWhiteSpace(x.ClosedReason);
            })
            .WithMessage("Closed reason is required when status is Won or Lost.");

        When(x => x.FollowUp is not null, () =>
        {
            RuleFor(x => x.FollowUp!.ActivityType).NotEmpty().MaximumLength(128);
            RuleFor(x => x.FollowUp!.FollowUpDate).NotEmpty();
            RuleFor(x => x.FollowUp!.Remarks).MaximumLength(4000);
            RuleFor(x => x.FollowUp!.Status).MaximumLength(64);
            RuleFor(x => x.FollowUp!)
                .Must(f => !f.NextFollowUpDate.HasValue || f.NextFollowUpDate >= f.FollowUpDate)
                .WithMessage("Next follow-up date must be on or after follow-up date.");
        });
    }
}

public sealed class UpdateOpportunityRequestValidator : AbstractValidator<UpdateOpportunityRequest>
{
    public UpdateOpportunityRequestValidator()
    {
        RuleFor(x => x.OpportunityValue).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Probability).InclusiveBetween(0, 100);
        RuleFor(x => x.Status).MaximumLength(64)
            .Must(s => string.IsNullOrWhiteSpace(s) || OpportunityStatuses.All.Contains(s!))
            .WithMessage("Invalid opportunity status.");
        RuleFor(x => x.Computations).MaximumLength(4000);
        RuleFor(x => x.Notes).MaximumLength(4000);
        RuleFor(x => x.ClosedReason).MaximumLength(1024);
        RuleFor(x => x.CurrencyCode).MaximumLength(16);
        RuleFor(x => x.Name).MaximumLength(256);
        RuleFor(x => x)
            .Must(x =>
            {
                if (string.IsNullOrWhiteSpace(x.Status) || !OpportunityStatuses.IsClosed(x.Status))
                {
                    return true;
                }

                return !string.IsNullOrWhiteSpace(x.ClosedReason);
            })
            .WithMessage("Closed reason is required when status is Won or Lost.");
    }
}

public sealed class CreateOpportunityFollowUpRequestValidator : AbstractValidator<CreateOpportunityFollowUpRequest>
{
    public CreateOpportunityFollowUpRequestValidator()
    {
        RuleFor(x => x.ActivityType).NotEmpty().MaximumLength(128);
        RuleFor(x => x.FollowUpDate).NotEmpty();
        RuleFor(x => x.Remarks).MaximumLength(4000);
        RuleFor(x => x.Status).MaximumLength(64);
        RuleFor(x => x)
            .Must(f => !f.NextFollowUpDate.HasValue || f.NextFollowUpDate >= f.FollowUpDate)
            .WithMessage("Next follow-up date must be on or after follow-up date.");
    }
}

public sealed class UpdateOpportunityFollowUpRequestValidator : AbstractValidator<UpdateOpportunityFollowUpRequest>
{
    public UpdateOpportunityFollowUpRequestValidator()
    {
        RuleFor(x => x.ActivityType).NotEmpty().MaximumLength(128);
        RuleFor(x => x.FollowUpDate).NotEmpty();
        RuleFor(x => x.Remarks).MaximumLength(4000);
        RuleFor(x => x.Status).MaximumLength(64);
        RuleFor(x => x)
            .Must(f => !f.NextFollowUpDate.HasValue || f.NextFollowUpDate >= f.FollowUpDate)
            .WithMessage("Next follow-up date must be on or after follow-up date.");
    }
}
