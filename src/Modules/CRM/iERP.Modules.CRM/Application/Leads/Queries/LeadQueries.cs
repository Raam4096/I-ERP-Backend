using AutoMapper;
using AutoMapper.QueryableExtensions;
using iERP.Application.Abstractions.Metadata;
using iERP.Modules.CRM.Application.Leads.Dtos;
using iERP.Modules.CRM.Infrastructure;
using iERP.SharedKernel.Exceptions;
using iERP.SharedKernel.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.CRM.Application.Leads.Queries;

public sealed record GetLeadByIdQuery(Guid Id) : IRequest<LeadDto>;

public sealed class GetLeadByIdQueryHandler : IRequestHandler<GetLeadByIdQuery, LeadDto>
{
    private readonly CrmDbContext _db;
    private readonly IMapper _mapper;
    private readonly ICustomFieldValueStore _customFieldValueStore;

    public GetLeadByIdQueryHandler(CrmDbContext db, IMapper mapper, ICustomFieldValueStore customFieldValueStore)
    {
        _db = db;
        _mapper = mapper;
        _customFieldValueStore = customFieldValueStore;
    }

    public async Task<LeadDto> Handle(GetLeadByIdQuery query, CancellationToken cancellationToken)
    {
        var lead = await _db.Leads
            .AsNoTracking()
            .Include(x => x.FollowUps)
                .ThenInclude(x => x.Attachments)
            .FirstOrDefaultAsync(x => x.Id == query.Id, cancellationToken)
            ?? throw new NotFoundException($"Lead '{query.Id}' was not found.");

        var dto = _mapper.Map<LeadDto>(lead);
        var customFields = await _customFieldValueStore.GetValuesAsync(
            LeadMetadata.EntityName,
            lead.Id,
            cancellationToken);
        return dto with { CustomFields = customFields.Count > 0 ? customFields : null };
    }
}

public sealed record GetLeadsQuery(
    int Page = 1,
    int PageSize = PaginationDefaults.DefaultPageSize,
    string? Search = null,
    string? Status = null,
    Guid? AssignedToUserId = null,
    string? SortBy = null,
    bool SortDescending = true) : IRequest<PagedResponse<LeadDto>>;

public sealed class GetLeadsQueryHandler : IRequestHandler<GetLeadsQuery, PagedResponse<LeadDto>>
{
    private readonly CrmDbContext _db;
    private readonly IMapper _mapper;

    public GetLeadsQueryHandler(CrmDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<PagedResponse<LeadDto>> Handle(GetLeadsQuery query, CancellationToken cancellationToken)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1
            ? PaginationDefaults.DefaultPageSize
            : Math.Min(query.PageSize, PaginationDefaults.MaxPageSize);

        var leads = _db.Leads.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLowerInvariant();
            leads = leads.Where(x =>
                x.CompanyName.ToLower().Contains(term) ||
                x.Email.ToLower().Contains(term) ||
                x.Phone.Contains(term) ||
                x.LeadNumber.ToLower().Contains(term) ||
                (x.ContactPerson != null && x.ContactPerson.ToLower().Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            leads = leads.Where(x => x.Status == query.Status);
        }

        if (query.AssignedToUserId.HasValue)
        {
            leads = leads.Where(x => x.AssignedToUserId == query.AssignedToUserId);
        }

        leads = (query.SortBy?.Trim().ToLowerInvariant()) switch
        {
            "companyname" => query.SortDescending
                ? leads.OrderByDescending(x => x.CompanyName)
                : leads.OrderBy(x => x.CompanyName),
            "status" => query.SortDescending
                ? leads.OrderByDescending(x => x.Status)
                : leads.OrderBy(x => x.Status),
            "leadnumber" => query.SortDescending
                ? leads.OrderByDescending(x => x.LeadNumber)
                : leads.OrderBy(x => x.LeadNumber),
            "email" => query.SortDescending
                ? leads.OrderByDescending(x => x.Email)
                : leads.OrderBy(x => x.Email),
            _ => query.SortDescending
                ? leads.OrderByDescending(x => x.CreatedAt)
                : leads.OrderBy(x => x.CreatedAt)
        };

        var total = await leads.CountAsync(cancellationToken);
        var items = await leads
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<LeadDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return PagedResponse<LeadDto>.Create(items, page, pageSize, total);
    }
}

public sealed record GetLeadTimelineQuery(Guid LeadId) : IRequest<IReadOnlyList<LeadFollowUpDto>>;

public sealed class GetLeadTimelineQueryHandler : IRequestHandler<GetLeadTimelineQuery, IReadOnlyList<LeadFollowUpDto>>
{
    private readonly CrmDbContext _db;
    private readonly IMapper _mapper;

    public GetLeadTimelineQueryHandler(CrmDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<LeadFollowUpDto>> Handle(GetLeadTimelineQuery query, CancellationToken cancellationToken)
    {
        var exists = await _db.Leads.AnyAsync(x => x.Id == query.LeadId, cancellationToken);
        if (!exists)
        {
            throw new NotFoundException($"Lead '{query.LeadId}' was not found.");
        }

        var followUps = await _db.LeadFollowUps
            .AsNoTracking()
            .Include(x => x.Attachments)
            .Where(x => x.LeadId == query.LeadId)
            .OrderByDescending(x => x.FollowUpDate)
            .ThenByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<LeadFollowUpDto>>(followUps);
    }
}
