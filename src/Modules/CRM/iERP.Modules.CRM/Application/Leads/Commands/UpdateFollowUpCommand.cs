using AutoMapper;
using iERP.Modules.CRM.Application.Leads.Dtos;
using iERP.Modules.CRM.Infrastructure;
using iERP.SharedKernel.Exceptions;
using iERP.SharedKernel.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace iERP.Modules.CRM.Application.Leads.Commands;

public sealed record UpdateFollowUpCommand(Guid Id, UpdateFollowUpRequest Request) : IRequest<LeadFollowUpDto>;

public sealed class UpdateFollowUpCommandHandler : IRequestHandler<UpdateFollowUpCommand, LeadFollowUpDto>
{
    private readonly CrmDbContext _db;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<UpdateFollowUpCommandHandler> _logger;

    public UpdateFollowUpCommandHandler(
        CrmDbContext db,
        IMapper mapper,
        ICurrentUser currentUser,
        ILogger<UpdateFollowUpCommandHandler> logger)
    {
        _db = db;
        _mapper = mapper;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<LeadFollowUpDto> Handle(UpdateFollowUpCommand command, CancellationToken cancellationToken)
    {
        var followUp = await _db.LeadFollowUps
            .Include(x => x.Attachments)
            .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken)
            ?? throw new NotFoundException($"Follow-up '{command.Id}' was not found.");

        var request = command.Request;
        followUp.Update(
            request.ActivityType,
            request.FollowUpDate,
            request.NextFollowUpDate,
            request.Remarks,
            request.Status);

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Follow-up updated {FollowUpId} by {UserId}", followUp.Id, _currentUser.UserId);
        return _mapper.Map<LeadFollowUpDto>(followUp);
    }
}
