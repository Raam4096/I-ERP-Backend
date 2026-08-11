using FluentValidation;

namespace iERP.Modules.CRM.Application.Validation;

/// <summary>
/// Placeholder demonstrating FluentValidation structure for future APIs.
/// </summary>
public sealed class CreateLeadPlaceholderRequest
{
    public string? LeadNumber { get; set; }
    public Guid SubsidiaryId { get; set; }
}

public sealed class CreateLeadPlaceholderValidator : AbstractValidator<CreateLeadPlaceholderRequest>
{
    public CreateLeadPlaceholderValidator()
    {
        RuleFor(x => x.LeadNumber).NotEmpty().MaximumLength(64);
        RuleFor(x => x.SubsidiaryId).NotEmpty();
    }
}
