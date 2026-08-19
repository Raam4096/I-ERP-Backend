namespace iERP.Modules.CRM.Application.Opportunities.Dtos;

public sealed class OpportunityFollowUpDto
{
    public Guid Id { get; init; }
    public Guid OpportunityId { get; init; }
    public string ActivityType { get; init; } = string.Empty;
    public DateTimeOffset FollowUpDate { get; init; }
    public DateTimeOffset? NextFollowUpDate { get; init; }
    public string? Remarks { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
    public Guid? CreatedBy { get; init; }
}

public sealed class OpportunityDto
{
    public Guid Id { get; init; }
    public string OpportunityNumber { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public Guid? LeadId { get; init; }
    public string? LeadNumber { get; set; }
    public Guid? SubsidiaryId { get; init; }
    public Guid? CustomerId { get; init; }
    public string Stage { get; init; } = string.Empty;
    public decimal OpportunityValue { get; init; }
    public string? CurrencyCode { get; init; }
    public DateOnly? ExpectedCloseDate { get; init; }
    public Guid? OwnerUserId { get; init; }
    public string Status { get; init; } = string.Empty;
    public int Probability { get; init; }
    public string? Computations { get; init; }
    public string? Notes { get; init; }
    public string? ClosedReason { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public Guid? CreatedBy { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
    public Guid? UpdatedBy { get; init; }
    public long Version { get; init; }
    public IReadOnlyList<OpportunityFollowUpDto>? FollowUps { get; init; }
}

public sealed record OpportunityFollowUpInputDto(
    string ActivityType,
    DateTimeOffset FollowUpDate,
    DateTimeOffset? NextFollowUpDate,
    string? Remarks,
    string? Status);

public sealed record ConvertLeadToOpportunityRequest(
    decimal OpportunityValue,
    int Probability,
    string? Status,
    string? Computations,
    string? Notes,
    string? ClosedReason,
    string? CurrencyCode,
    DateOnly? ExpectedCloseDate,
    Guid? OwnerUserId,
    OpportunityFollowUpInputDto? FollowUp);

public sealed record UpdateOpportunityRequest(
    decimal OpportunityValue,
    int Probability,
    string? Status,
    string? Computations,
    string? Notes,
    string? ClosedReason,
    string? CurrencyCode,
    DateOnly? ExpectedCloseDate,
    Guid? OwnerUserId,
    string? Name);

public sealed record CreateOpportunityFollowUpRequest(
    string ActivityType,
    DateTimeOffset FollowUpDate,
    DateTimeOffset? NextFollowUpDate,
    string? Remarks,
    string? Status);

public sealed record UpdateOpportunityFollowUpRequest(
    string ActivityType,
    DateTimeOffset FollowUpDate,
    DateTimeOffset? NextFollowUpDate,
    string? Remarks,
    string? Status);

public sealed class CrmHistoryItemDto
{
    public Guid Id { get; init; }
    public string Source { get; init; } = string.Empty;
    public Guid ParentId { get; init; }
    public string? ParentNumber { get; init; }
    public string ActivityType { get; init; } = string.Empty;
    public DateTimeOffset FollowUpDate { get; init; }
    public DateTimeOffset? NextFollowUpDate { get; init; }
    public string? Remarks { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
    public Guid? CreatedBy { get; init; }
}
