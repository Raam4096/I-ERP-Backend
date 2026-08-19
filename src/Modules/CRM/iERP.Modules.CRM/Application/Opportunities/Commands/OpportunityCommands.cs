using AutoMapper;
using iERP.Modules.CRM.Application.Opportunities.Dtos;
using iERP.Modules.CRM.Infrastructure;
using iERP.SharedKernel.Exceptions;
using iERP.SharedKernel.Security;
using iERP.SharedKernel.Time;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace iERP.Modules.CRM.Application.Opportunities.Commands;

public sealed record UpdateOpportunityCommand(Guid Id, UpdateOpportunityRequest Request) : IRequest<OpportunityDto>;

public sealed class UpdateOpportunityCommandHandler : IRequestHandler<UpdateOpportunityCommand, OpportunityDto>
{
    private readonly CrmDbContext _db;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<UpdateOpportunityCommandHandler> _logger;

    public UpdateOpportunityCommandHandler(
        CrmDbContext db,
        IMapper mapper,
        ICurrentUser currentUser,
        ILogger<UpdateOpportunityCommandHandler> logger)
    {
        _db = db;
        _mapper = mapper;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<OpportunityDto> Handle(UpdateOpportunityCommand command, CancellationToken cancellationToken)
    {
        var opportunity = await _db.Opportunities
            .Include(x => x.FollowUps)
            .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken)
            ?? throw new NotFoundException($"Opportunity '{command.Id}' was not found.");

        var request = command.Request;
        opportunity.Update(
            request.OpportunityValue,
            request.Probability,
            request.Status,
            request.Computations,
            request.Notes,
            request.ClosedReason,
            request.CurrencyCode,
            request.ExpectedCloseDate,
            request.OwnerUserId,
            request.Name);

        await _db.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Opportunity updated {OpportunityId} by {UserId}", opportunity.Id, _currentUser.UserId);

        return await MapWithLeadNumberAsync(opportunity, cancellationToken);
    }

    private async Task<OpportunityDto> MapWithLeadNumberAsync(
        Domain.Opportunity opportunity,
        CancellationToken cancellationToken)
    {
        var dto = _mapper.Map<OpportunityDto>(opportunity);
        if (!opportunity.LeadId.HasValue)
        {
            return dto;
        }

        var leadNumber = await _db.Leads.AsNoTracking()
            .Where(x => x.Id == opportunity.LeadId)
            .Select(x => x.LeadNumber)
            .FirstOrDefaultAsync(cancellationToken);

        dto.LeadNumber = leadNumber;
        return dto;
    }
}

public sealed record DiscardOpportunityCommand(Guid Id) : IRequest<OpportunityDto>;

public sealed class DiscardOpportunityCommandHandler : IRequestHandler<DiscardOpportunityCommand, OpportunityDto>
{
    private readonly CrmDbContext _db;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<DiscardOpportunityCommandHandler> _logger;

    public DiscardOpportunityCommandHandler(
        CrmDbContext db,
        IMapper mapper,
        ICurrentUser currentUser,
        ILogger<DiscardOpportunityCommandHandler> logger)
    {
        _db = db;
        _mapper = mapper;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<OpportunityDto> Handle(DiscardOpportunityCommand command, CancellationToken cancellationToken)
    {
        var opportunity = await _db.Opportunities.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken)
            ?? throw new NotFoundException($"Opportunity '{command.Id}' was not found.");

        opportunity.Discard();
        await _db.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Opportunity discarded {OpportunityId} by {UserId}", opportunity.Id, _currentUser.UserId);
        return _mapper.Map<OpportunityDto>(opportunity);
    }
}

public sealed record RestoreOpportunityCommand(Guid Id) : IRequest<OpportunityDto>;

public sealed class RestoreOpportunityCommandHandler : IRequestHandler<RestoreOpportunityCommand, OpportunityDto>
{
    private readonly CrmDbContext _db;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<RestoreOpportunityCommandHandler> _logger;

    public RestoreOpportunityCommandHandler(
        CrmDbContext db,
        IMapper mapper,
        ICurrentUser currentUser,
        ILogger<RestoreOpportunityCommandHandler> logger)
    {
        _db = db;
        _mapper = mapper;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<OpportunityDto> Handle(RestoreOpportunityCommand command, CancellationToken cancellationToken)
    {
        var opportunity = await _db.Opportunities.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken)
            ?? throw new NotFoundException($"Opportunity '{command.Id}' was not found.");

        opportunity.Restore();
        await _db.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Opportunity restored {OpportunityId} by {UserId}", opportunity.Id, _currentUser.UserId);
        return _mapper.Map<OpportunityDto>(opportunity);
    }
}

public sealed record DeleteOpportunityCommand(Guid Id) : IRequest;

public sealed class DeleteOpportunityCommandHandler : IRequestHandler<DeleteOpportunityCommand>
{
    private readonly CrmDbContext _db;
    private readonly ICurrentUser _currentUser;
    private readonly IClock _clock;
    private readonly ILogger<DeleteOpportunityCommandHandler> _logger;

    public DeleteOpportunityCommandHandler(
        CrmDbContext db,
        ICurrentUser currentUser,
        IClock clock,
        ILogger<DeleteOpportunityCommandHandler> logger)
    {
        _db = db;
        _currentUser = currentUser;
        _clock = clock;
        _logger = logger;
    }

    public async Task Handle(DeleteOpportunityCommand command, CancellationToken cancellationToken)
    {
        var opportunity = await _db.Opportunities.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken)
            ?? throw new NotFoundException($"Opportunity '{command.Id}' was not found.");

        opportunity.MarkDeleted(_currentUser.UserId, _clock.UtcNow);
        await _db.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Opportunity soft-deleted {OpportunityId} by {UserId}", opportunity.Id, _currentUser.UserId);
    }
}

public sealed record CreateOpportunityFollowUpCommand(Guid OpportunityId, CreateOpportunityFollowUpRequest Request)
    : IRequest<OpportunityFollowUpDto>;

public sealed class CreateOpportunityFollowUpCommandHandler
    : IRequestHandler<CreateOpportunityFollowUpCommand, OpportunityFollowUpDto>
{
    private readonly CrmDbContext _db;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<CreateOpportunityFollowUpCommandHandler> _logger;

    public CreateOpportunityFollowUpCommandHandler(
        CrmDbContext db,
        IMapper mapper,
        ICurrentUser currentUser,
        ILogger<CreateOpportunityFollowUpCommandHandler> logger)
    {
        _db = db;
        _mapper = mapper;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<OpportunityFollowUpDto> Handle(
        CreateOpportunityFollowUpCommand command,
        CancellationToken cancellationToken)
    {
        var opportunity = await _db.Opportunities
            .Include(x => x.FollowUps)
            .FirstOrDefaultAsync(x => x.Id == command.OpportunityId, cancellationToken)
            ?? throw new NotFoundException($"Opportunity '{command.OpportunityId}' was not found.");

        var request = command.Request;
        var followUp = opportunity.AddFollowUp(
            request.ActivityType,
            request.FollowUpDate,
            request.NextFollowUpDate,
            request.Remarks,
            request.Status);

        await _db.SaveChangesAsync(cancellationToken);
        _logger.LogInformation(
            "Follow-up {FollowUpId} added to opportunity {OpportunityId} by {UserId}",
            followUp.Id,
            opportunity.Id,
            _currentUser.UserId);

        return _mapper.Map<OpportunityFollowUpDto>(followUp);
    }
}

public sealed record UpdateOpportunityFollowUpCommand(Guid Id, UpdateOpportunityFollowUpRequest Request)
    : IRequest<OpportunityFollowUpDto>;

public sealed class UpdateOpportunityFollowUpCommandHandler
    : IRequestHandler<UpdateOpportunityFollowUpCommand, OpportunityFollowUpDto>
{
    private readonly CrmDbContext _db;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<UpdateOpportunityFollowUpCommandHandler> _logger;

    public UpdateOpportunityFollowUpCommandHandler(
        CrmDbContext db,
        IMapper mapper,
        ICurrentUser currentUser,
        ILogger<UpdateOpportunityFollowUpCommandHandler> logger)
    {
        _db = db;
        _mapper = mapper;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<OpportunityFollowUpDto> Handle(
        UpdateOpportunityFollowUpCommand command,
        CancellationToken cancellationToken)
    {
        var followUp = await _db.OpportunityFollowUps
            .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken)
            ?? throw new NotFoundException($"Opportunity follow-up '{command.Id}' was not found.");

        var request = command.Request;
        followUp.Update(
            request.ActivityType,
            request.FollowUpDate,
            request.NextFollowUpDate,
            request.Remarks,
            request.Status);

        await _db.SaveChangesAsync(cancellationToken);
        _logger.LogInformation(
            "Opportunity follow-up updated {FollowUpId} by {UserId}",
            followUp.Id,
            _currentUser.UserId);

        return _mapper.Map<OpportunityFollowUpDto>(followUp);
    }
}
