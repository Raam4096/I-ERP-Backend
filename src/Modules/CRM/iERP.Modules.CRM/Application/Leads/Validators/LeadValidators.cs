using FluentValidation;
using iERP.Modules.CRM.Application.Common.Validation;
using iERP.Modules.CRM.Application.Leads.Dtos;

namespace iERP.Modules.CRM.Application.Leads.Validators;

public sealed class CreateLeadRequestValidator : AbstractValidator<CreateLeadRequest>
{
    public CreateLeadRequestValidator()
    {
        RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.ContactPerson).MaximumLength(256);
        RuleFor(x => x.Industry).MaximumLength(128);
        RuleFor(x => x.Address).MaximumLength(1024);
        RuleFor(x => x.CompanySize).MaximumLength(64);
        RuleFor(x => x.LeadSource).MaximumLength(128);
        RuleFor(x => x.ProjectDescription).MaximumLength(4000);
        RuleFor(x => x.ProjectType).MaximumLength(128);
        RuleFor(x => x.Status).MaximumLength(64);
        RuleFor(x => x.Subsidiary).MaximumLength(256);
        RuleFor(x => x.Website)
            .MaximumLength(512)
            .Must(UrlValidation.BeValidHttpUrlOrEmpty).WithMessage("Website must be a valid URL.");
        RuleFor(x => x.Notes).MaximumLength(4000);
        RuleFor(x => x.AnnualRevenue).GreaterThanOrEqualTo(0).When(x => x.AnnualRevenue.HasValue);

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

public sealed class UpdateLeadRequestValidator : AbstractValidator<UpdateLeadRequest>
{
    public UpdateLeadRequestValidator()
    {
        RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.ContactPerson).MaximumLength(256);
        RuleFor(x => x.Industry).MaximumLength(128);
        RuleFor(x => x.Address).MaximumLength(1024);
        RuleFor(x => x.CompanySize).MaximumLength(64);
        RuleFor(x => x.LeadSource).MaximumLength(128);
        RuleFor(x => x.ProjectDescription).MaximumLength(4000);
        RuleFor(x => x.ProjectType).MaximumLength(128);
        RuleFor(x => x.Status).MaximumLength(64);
        RuleFor(x => x.Subsidiary).MaximumLength(256);
        RuleFor(x => x.Website)
            .MaximumLength(512)
            .Must(UrlValidation.BeValidHttpUrlOrEmpty).WithMessage("Website must be a valid URL.");
        RuleFor(x => x.Notes).MaximumLength(4000);
        RuleFor(x => x.AnnualRevenue).GreaterThanOrEqualTo(0).When(x => x.AnnualRevenue.HasValue);
    }
}

public sealed class CreateFollowUpRequestValidator : AbstractValidator<CreateFollowUpRequest>
{
    public CreateFollowUpRequestValidator()
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

public sealed class UpdateFollowUpRequestValidator : AbstractValidator<UpdateFollowUpRequest>
{
    public UpdateFollowUpRequestValidator()
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
