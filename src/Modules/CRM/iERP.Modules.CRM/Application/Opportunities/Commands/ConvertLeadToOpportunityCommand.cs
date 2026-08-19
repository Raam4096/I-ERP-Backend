using AutoMapper;
using iERP.Modules.CRM.Application.Opportunities.Dtos;
using iERP.Modules.CRM.Application.Opportunities.Services;
using iERP.Modules.CRM.Domain;
using iERP.Modules.CRM.Infrastructure;
using iERP.SharedKernel.Exceptions;
using iERP.SharedKernel.Security;
using iERP.SharedKernel.Tenancy;
using iERP.SharedKernel.Time;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace iERP.Modules.CRM.Application.Opportunities.Commands;

public sealed record ConvertLeadToOpportunityCommand(Guid LeadId, ConvertLeadToOpportunityRequest Request)
    : IRequest<OpportunityDto>;

public sealed class ConvertLeadToOpportunityCommandHandler
    : IRequestHandler<ConvertLeadToOpportunityCommand, OpportunityDto>
{
    private readonly CrmDbContext _db;
    private readonly ITenantContext _tenantContext;
    private readonly IOpportunityNumberGenerator _numberGenerator;
    private readonly IClock _clock;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;
    private readonly ILogger<ConvertLeadToOpportunityCommandHandler> _logger;

    public ConvertLeadToOpportunityCommandHandler(
        CrmDbContext db,
        ITenantContext tenantContext,
        IOpportunityNumberGenerator numberGenerator,
        IClock clock,
        ICurrentUser currentUser,
        IMapper mapper,
        ILogger<ConvertLeadToOpportunityCommandHandler> logger)
    {
        _db = db;
        _tenantContext = tenantContext;
        _numberGenerator = numberGenerator;
        _clock = clock;
        _currentUser = currentUser;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<OpportunityDto> Handle(
        ConvertLeadToOpportunityCommand command,
        CancellationToken cancellationToken)
    {
        if (!_tenantContext.HasTenant)
        {
            throw new ForbiddenException("Tenant context is required.", ErrorCodes.TenantNotFound);
        }

        var lead = await _db.Leads.FirstOrDefaultAsync(x => x.Id == command.LeadId, cancellationToken)
            ?? throw new NotFoundException($"Lead '{command.LeadId}' was not found.");

        var request = command.Request;
        var tenantId = _tenantContext.TenantId!.Value;
        var number = await _numberGenerator.GenerateAsync(tenantId, cancellationToken);

        var opportunity = Opportunity.CreateFromLead(
            tenantId,
            number,
            lead,
            request.OpportunityValue,
            request.Probability,
            request.Status,
            request.Computations,
            request.Notes,
            request.ClosedReason,
            request.CurrencyCode,
            request.ExpectedCloseDate,
            request.OwnerUserId);

        lead.MarkConverted(opportunity.Id, _clock.UtcNow);

        if (request.FollowUp is not null)
        {
            opportunity.AddFollowUp(
                request.FollowUp.ActivityType,
                request.FollowUp.FollowUpDate,
                request.FollowUp.NextFollowUpDate,
                request.FollowUp.Remarks,
                request.FollowUp.Status);
        }

        _db.Opportunities.Add(opportunity);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Lead {LeadId} converted to opportunity {OpportunityId} by {UserId}",
            lead.Id,
            opportunity.Id,
            _currentUser.UserId);

        var dto = _mapper.Map<OpportunityDto>(opportunity);
        dto.LeadNumber = lead.LeadNumber;
        return dto;
    }
}
