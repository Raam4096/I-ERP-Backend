using AutoMapper;
using iERP.Modules.CRM.Application.Leads.Dtos;
using iERP.Modules.CRM.Infrastructure;
using iERP.SharedKernel.Exceptions;
using iERP.SharedKernel.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace iERP.Modules.CRM.Application.Leads.Commands;

public sealed record UpdateLeadCommand(Guid Id, UpdateLeadRequest Request) : IRequest<LeadDto>;

public sealed class UpdateLeadCommandHandler : IRequestHandler<UpdateLeadCommand, LeadDto>
{
    private readonly CrmDbContext _db;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<UpdateLeadCommandHandler> _logger;

    public UpdateLeadCommandHandler(
        CrmDbContext db,
        IMapper mapper,
        ICurrentUser currentUser,
        ILogger<UpdateLeadCommandHandler> logger)
    {
        _db = db;
        _mapper = mapper;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<LeadDto> Handle(UpdateLeadCommand command, CancellationToken cancellationToken)
    {
        var lead = await _db.Leads.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken)
            ?? throw new NotFoundException($"Lead '{command.Id}' was not found.");

        var request = command.Request;
        var email = request.Email.Trim().ToLowerInvariant();
        var phone = request.Phone.Trim();

        var duplicate = await _db.Leads.AnyAsync(
            x => x.Id != lead.Id && (x.Email == email || x.Phone == phone),
            cancellationToken);

        if (duplicate)
        {
            throw new BusinessRuleException(ErrorCodes.DuplicateRecord, "A lead with the same email or phone already exists.");
        }

        lead.Update(
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

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Lead updated {LeadId} by {UserId}", lead.Id, _currentUser.UserId);
        return _mapper.Map<LeadDto>(lead);
    }
}
