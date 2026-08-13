using AutoMapper;
using iERP.Modules.CRM.Application.Leads.Dtos;
using iERP.Modules.CRM.Infrastructure;
using iERP.SharedKernel.Exceptions;
using iERP.SharedKernel.Security;
using iERP.SharedKernel.Time;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace iERP.Modules.CRM.Application.Leads.Commands;

public sealed record CreateFollowUpCommand(Guid LeadId, CreateFollowUpRequest Request) : IRequest<LeadFollowUpDto>;

public sealed class CreateFollowUpCommandHandler : IRequestHandler<CreateFollowUpCommand, LeadFollowUpDto>
{
    private readonly CrmDbContext _db;
    private readonly IMapper _mapper;
    private readonly IClock _clock;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<CreateFollowUpCommandHandler> _logger;

    public CreateFollowUpCommandHandler(
        CrmDbContext db,
        IMapper mapper,
        IClock clock,
        ICurrentUser currentUser,
        ILogger<CreateFollowUpCommandHandler> logger)
    {
        _db = db;
        _mapper = mapper;
        _clock = clock;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<LeadFollowUpDto> Handle(CreateFollowUpCommand command, CancellationToken cancellationToken)
    {
        var lead = await _db.Leads
            .Include(x => x.FollowUps)
            .FirstOrDefaultAsync(x => x.Id == command.LeadId, cancellationToken)
            ?? throw new NotFoundException($"Lead '{command.LeadId}' was not found.");

        var request = command.Request;
        var followUp = lead.AddFollowUp(
            request.ActivityType,
            request.FollowUpDate,
            request.NextFollowUpDate,
            request.Remarks,
            request.Status);

        if (request.Attachments is { Count: > 0 })
        {
            foreach (var attachment in request.Attachments)
            {
                followUp.AddAttachment(
                    attachment.FileName,
                    attachment.FilePath,
                    attachment.ContentType ?? "application/octet-stream",
                    attachment.FileSize,
                    _clock.UtcNow);
            }
        }

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Follow-up added {FollowUpId} to lead {LeadId} by {UserId}",
            followUp.Id,
            lead.Id,
            _currentUser.UserId);

        return _mapper.Map<LeadFollowUpDto>(followUp);
    }
}
