using AutoMapper;
using iERP.Application.Abstractions.Metadata;
using iERP.Modules.CRM.Application.Leads.Dtos;
using iERP.Modules.CRM.Application.Leads.Services;
using iERP.Modules.CRM.Domain;
using iERP.Modules.CRM.Infrastructure;
using iERP.SharedKernel.Exceptions;
using iERP.SharedKernel.Security;
using iERP.SharedKernel.Tenancy;
using iERP.SharedKernel.Time;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace iERP.Modules.CRM.Application.Leads.Commands;

public sealed record CreateLeadCommand(CreateLeadRequest Request) : IRequest<LeadDto>;

public sealed class CreateLeadCommandHandler : IRequestHandler<CreateLeadCommand, LeadDto>
{
    private readonly CrmDbContext _db;
    private readonly ITenantContext _tenantContext;
    private readonly ILeadNumberGenerator _leadNumberGenerator;
    private readonly ICurrentUser _currentUser;
    private readonly IClock _clock;
    private readonly IMapper _mapper;
    private readonly ICustomFieldValueStore _customFieldValueStore;
    private readonly ILogger<CreateLeadCommandHandler> _logger;

    public CreateLeadCommandHandler(
        CrmDbContext db,
        ITenantContext tenantContext,
        ILeadNumberGenerator leadNumberGenerator,
        ICurrentUser currentUser,
        IClock clock,
        IMapper mapper,
        ICustomFieldValueStore customFieldValueStore,
        ILogger<CreateLeadCommandHandler> logger)
    {
        _db = db;
        _tenantContext = tenantContext;
        _leadNumberGenerator = leadNumberGenerator;
        _currentUser = currentUser;
        _clock = clock;
        _mapper = mapper;
        _customFieldValueStore = customFieldValueStore;
        _logger = logger;
    }

    public async Task<LeadDto> Handle(CreateLeadCommand command, CancellationToken cancellationToken)
    {
        if (!_tenantContext.HasTenant)
        {
            throw new ForbiddenException("Tenant context is required.", ErrorCodes.TenantNotFound);
        }

        var request = command.Request;
        var tenantId = _tenantContext.TenantId!.Value;
        var email = request.Email.Trim().ToLowerInvariant();
        var phone = request.Phone.Trim();

        var duplicate = await _db.Leads.AnyAsync(
            x => x.Email == email || x.Phone == phone,
            cancellationToken);

        if (duplicate)
        {
            _logger.LogWarning("Duplicate lead rejected for email/phone. Tenant {TenantId}", tenantId);
            throw new BusinessRuleException(ErrorCodes.DuplicateRecord, "A lead with the same email or phone already exists.");
        }

        var leadNumber = await _leadNumberGenerator.GenerateAsync(tenantId, cancellationToken);
        var lead = Lead.Create(
            tenantId,
            leadNumber,
            request.CompanyName,
            phone,
            email,
            request.ContactPerson,
            request.Industry,
            request.Address,
            request.AnnualRevenue,
            request.AssignedTo,
            request.CompanySize,
            request.LeadSource,
            request.ProjectDescription,
            request.ProjectType,
            request.Status,
            request.Subsidiary,
            request.SubsidiaryId,
            request.Website,
            request.Notes);

        if (request.FollowUp is not null)
        {
            var followUp = lead.AddFollowUp(
                request.FollowUp.ActivityType,
                request.FollowUp.FollowUpDate,
                request.FollowUp.NextFollowUpDate,
                request.FollowUp.Remarks,
                request.FollowUp.Status);

            if (request.FollowUp.Attachments is { Count: > 0 })
            {
                foreach (var attachment in request.FollowUp.Attachments)
                {
                    followUp.AddAttachment(
                        attachment.FileName,
                        attachment.FilePath,
                        attachment.ContentType ?? "application/octet-stream",
                        attachment.FileSize,
                        _clock.UtcNow);
                }
            }
        }

        _db.Leads.Add(lead);
        await _db.SaveChangesAsync(cancellationToken);

        if (request.CustomFields is { Count: > 0 })
        {
            await _customFieldValueStore.UpsertValuesAsync(
                LeadMetadata.EntityName,
                lead.Id,
                request.CustomFields,
                cancellationToken);
        }

        _logger.LogInformation(
            "Lead created {LeadId} {LeadNumber} by {UserId}",
            lead.Id,
            lead.LeadNumber,
            _currentUser.UserId);

        var dto = _mapper.Map<LeadDto>(lead);
        var customFields = request.CustomFields is { Count: > 0 }
            ? await _customFieldValueStore.GetValuesAsync(LeadMetadata.EntityName, lead.Id, cancellationToken)
            : null;
        return dto with { CustomFields = customFields };
    }
}
