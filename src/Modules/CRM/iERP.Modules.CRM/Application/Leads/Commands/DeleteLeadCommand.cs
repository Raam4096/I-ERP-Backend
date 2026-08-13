using iERP.Modules.CRM.Infrastructure;
using iERP.SharedKernel.Exceptions;
using iERP.SharedKernel.Security;
using iERP.SharedKernel.Time;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace iERP.Modules.CRM.Application.Leads.Commands;

public sealed record DeleteLeadCommand(Guid Id) : IRequest<Unit>;

public sealed class DeleteLeadCommandHandler : IRequestHandler<DeleteLeadCommand, Unit>
{
    private readonly CrmDbContext _db;
    private readonly ICurrentUser _currentUser;
    private readonly IClock _clock;
    private readonly ILogger<DeleteLeadCommandHandler> _logger;

    public DeleteLeadCommandHandler(
        CrmDbContext db,
        ICurrentUser currentUser,
        IClock clock,
        ILogger<DeleteLeadCommandHandler> logger)
    {
        _db = db;
        _currentUser = currentUser;
        _clock = clock;
        _logger = logger;
    }

    public async Task<Unit> Handle(DeleteLeadCommand command, CancellationToken cancellationToken)
    {
        var lead = await _db.Leads.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken)
            ?? throw new NotFoundException($"Lead '{command.Id}' was not found.");

        lead.MarkDeleted(_currentUser.UserId, _clock.UtcNow);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Lead soft-deleted {LeadId} by {UserId}", lead.Id, _currentUser.UserId);
        return Unit.Value;
    }
}
