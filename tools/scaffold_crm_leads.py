#!/usr/bin/env python3
"""Generate CRM Lead application layer files."""
from pathlib import Path
from textwrap import dedent

ROOT = Path(r"e:\Work\ERP")
BASE = ROOT / "src/Modules/CRM/iERP.Modules.CRM"

def write(rel: str, content: str) -> None:
    path = BASE / rel
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(dedent(content).lstrip("\n").replace("\r\n", "\n"), encoding="utf-8")
    print(rel)

# DTOs
write("Application/Leads/Dtos/LeadDtos.cs", r'''
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
    IReadOnlyList<LeadFollowUpDto>? FollowUps = null);

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
    FollowUpInputDto? FollowUp);

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
    string? Notes);

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
''')

write("Application/Common/ValidationBehavior.cs", r'''
using FluentValidation;
using MediatR;
using ValidationException = iERP.SharedKernel.Exceptions.ValidationException;

namespace iERP.Modules.CRM.Application.Common;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var failures = (await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken))))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
        {
            return await next();
        }

        var errors = failures.Select(f => f.ErrorMessage).Distinct().ToArray();
        var field = failures[0].PropertyName;
        throw new ValidationException("One or more validation errors occurred.", field, errors);
    }
}
''')

write("Application/Leads/Services/ILeadNumberGenerator.cs", r'''
namespace iERP.Modules.CRM.Application.Leads.Services;

public interface ILeadNumberGenerator
{
    Task<string> GenerateAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
''')

write("Application/Leads/Services/LeadNumberGenerator.cs", r'''
using iERP.Modules.CRM.Infrastructure;
using iERP.SharedKernel.Time;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.CRM.Application.Leads.Services;

public sealed class LeadNumberGenerator : ILeadNumberGenerator
{
    private readonly CrmDbContext _db;
    private readonly IClock _clock;

    public LeadNumberGenerator(CrmDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<string> GenerateAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var year = _clock.UtcNow.Year;
        var prefix = $"LEAD-{year}-";

        var last = await _db.Leads
            .IgnoreQueryFilters()
            .Where(x => x.TenantId == tenantId && !x.IsDeleted && x.LeadNumber.StartsWith(prefix))
            .OrderByDescending(x => x.LeadNumber)
            .Select(x => x.LeadNumber)
            .FirstOrDefaultAsync(cancellationToken);

        var next = 1;
        if (!string.IsNullOrWhiteSpace(last) && last.Length >= prefix.Length + 6)
        {
            var suffix = last[prefix.Length..];
            if (int.TryParse(suffix, out var parsed))
            {
                next = parsed + 1;
            }
        }

        return $"{prefix}{next:D6}";
    }
}
''')

write("Application/Mapping/CrmMappingProfile.cs", r'''
using AutoMapper;
using iERP.Modules.CRM.Application.Leads.Dtos;
using iERP.Modules.CRM.Domain;

namespace iERP.Modules.CRM.Application.Mapping;

public sealed class CrmMappingProfile : Profile
{
    public CrmMappingProfile()
    {
        CreateMap<LeadAttachment, LeadAttachmentDto>();
        CreateMap<LeadFollowUp, LeadFollowUpDto>();
        CreateMap<Lead, LeadDto>()
            .ForMember(d => d.FollowUps, opt => opt.MapFrom(s => s.FollowUps));
    }
}
''')

print("base app files done")
