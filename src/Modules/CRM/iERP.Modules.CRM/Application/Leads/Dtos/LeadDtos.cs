namespace iERP.Modules.CRM.Application.Leads.Dtos;

public sealed record LeadAttachmentDto(
    Guid Id,
    string FileName,
    string FilePath,
    string ContentType,
    long FileSize,
    DateTimeOffset CreatedAt);

public sealed record LeadFollowUpDto(
    Guid Id,
    Guid LeadId,
    string ActivityType,
    DateTimeOffset FollowUpDate,
    DateTimeOffset? NextFollowUpDate,
    string? Remarks,
    string Status,
    DateTimeOffset CreatedAt,
    Guid? CreatedBy,
    IReadOnlyList<LeadAttachmentDto> Attachments);

public sealed record LeadDto(
    Guid Id,
    string LeadNumber,
    string CompanyName,
    string? ContactPerson,
    string Phone,
    string Email,
    string? Industry,
    string? Address,
    decimal? AnnualRevenue,
    Guid? AssignedToUserId,
    string? CompanySize,
    string? LeadSource,
    string? ProjectDescription,
    string? ProjectType,
    string Status,
    string? Subsidiary,
    Guid? SubsidiaryId,
    string? Website,
    string? Notes,
    DateTimeOffset CreatedAt,
    Guid? CreatedBy,
    DateTimeOffset? UpdatedAt,
    Guid? UpdatedBy,
    long Version,
    IReadOnlyList<LeadFollowUpDto>? FollowUps = null,
    IReadOnlyDictionary<string, object?>? CustomFields = null);

public sealed record AttachmentInputDto(
    string FileName,
    string FilePath,
    string? ContentType,
    long FileSize);

public sealed record FollowUpInputDto(
    string ActivityType,
    DateTimeOffset FollowUpDate,
    DateTimeOffset? NextFollowUpDate,
    string? Remarks,
    string? Status,
    IReadOnlyList<AttachmentInputDto>? Attachments);

public sealed record CreateLeadRequest(
    string CompanyName,
    string? ContactPerson,
    string Phone,
    string Email,
    string? Industry,
    string? Address,
    decimal? AnnualRevenue,
    Guid? AssignedTo,
    string? CompanySize,
    string? LeadSource,
    string? ProjectDescription,
    string? ProjectType,
    string? Status,
    string? Subsidiary,
    Guid? SubsidiaryId,
    string? Website,
    string? Notes,
    FollowUpInputDto? FollowUp,
    IReadOnlyDictionary<string, object?>? CustomFields = null);

public sealed record UpdateLeadRequest(
    string CompanyName,
    string? ContactPerson,
    string Phone,
    string Email,
    string? Industry,
    string? Address,
    decimal? AnnualRevenue,
    Guid? AssignedTo,
    string? CompanySize,
    string? LeadSource,
    string? ProjectDescription,
    string? ProjectType,
    string? Status,
    string? Subsidiary,
    Guid? SubsidiaryId,
    string? Website,
    string? Notes,
    IReadOnlyDictionary<string, object?>? CustomFields = null);

public sealed record CreateFollowUpRequest(
    string ActivityType,
    DateTimeOffset FollowUpDate,
    DateTimeOffset? NextFollowUpDate,
    string? Remarks,
    string? Status,
    IReadOnlyList<AttachmentInputDto>? Attachments);

public sealed record UpdateFollowUpRequest(
    string ActivityType,
    DateTimeOffset FollowUpDate,
    DateTimeOffset? NextFollowUpDate,
    string? Remarks,
    string? Status);
