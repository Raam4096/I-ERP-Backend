using AutoMapper;
using iERP.Modules.CRM.Application.Opportunities.Dtos;
using iERP.Modules.CRM.Infrastructure;
using iERP.SharedKernel.Exceptions;
using iERP.SharedKernel.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.CRM.Application.Opportunities.Queries;

public sealed record GetOpportunityByIdQuery(Guid Id) : IRequest<OpportunityDto>;

public sealed class GetOpportunityByIdQueryHandler : IRequestHandler<GetOpportunityByIdQuery, OpportunityDto>
{
    private readonly CrmDbContext _db;
    private readonly IMapper _mapper;

    public GetOpportunityByIdQueryHandler(CrmDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<OpportunityDto> Handle(GetOpportunityByIdQuery query, CancellationToken cancellationToken)
    {
        var opportunity = await _db.Opportunities
            .AsNoTracking()
            .Include(x => x.FollowUps)
            .FirstOrDefaultAsync(x => x.Id == query.Id, cancellationToken)
            ?? throw new NotFoundException($"Opportunity '{query.Id}' was not found.");

        var dto = _mapper.Map<OpportunityDto>(opportunity);
        if (opportunity.LeadId.HasValue)
        {
            var leadNumber = await _db.Leads.AsNoTracking()
                .Where(x => x.Id == opportunity.LeadId)
                .Select(x => x.LeadNumber)
                .FirstOrDefaultAsync(cancellationToken);
            dto.LeadNumber = leadNumber;
        }

        return dto;
    }
}

public sealed record GetOpportunitiesQuery(
    int Page,
    int PageSize,
    string? Search,
    string? Status,
    Guid? LeadId,
    Guid? OwnerUserId,
    string? SortBy,
    bool SortDescending) : IRequest<PagedResponse<OpportunityDto>>;

public sealed class GetOpportunitiesQueryHandler : IRequestHandler<GetOpportunitiesQuery, PagedResponse<OpportunityDto>>
{
    private readonly CrmDbContext _db;
    private readonly IMapper _mapper;

    public GetOpportunitiesQueryHandler(CrmDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<PagedResponse<OpportunityDto>> Handle(
        GetOpportunitiesQuery query,
        CancellationToken cancellationToken)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? PaginationDefaults.DefaultPageSize : Math.Min(query.PageSize, 100);

        var opportunities = _db.Opportunities.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLowerInvariant();
            opportunities = opportunities.Where(x =>
                x.OpportunityNumber.ToLower().Contains(term) ||
                x.Name.ToLower().Contains(term) ||
                (x.Notes != null && x.Notes.ToLower().Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            opportunities = opportunities.Where(x => x.Status == query.Status);
        }

        if (query.LeadId.HasValue)
        {
            opportunities = opportunities.Where(x => x.LeadId == query.LeadId);
        }

        if (query.OwnerUserId.HasValue)
        {
            opportunities = opportunities.Where(x => x.OwnerUserId == query.OwnerUserId);
        }

        opportunities = (query.SortBy?.ToLowerInvariant()) switch
        {
            "name" => query.SortDescending
                ? opportunities.OrderByDescending(x => x.Name)
                : opportunities.OrderBy(x => x.Name),
            "status" => query.SortDescending
                ? opportunities.OrderByDescending(x => x.Status)
                : opportunities.OrderBy(x => x.Status),
            "opportunitynumber" => query.SortDescending
                ? opportunities.OrderByDescending(x => x.OpportunityNumber)
                : opportunities.OrderBy(x => x.OpportunityNumber),
            "opportunityvalue" => query.SortDescending
                ? opportunities.OrderByDescending(x => x.OpportunityValue)
                : opportunities.OrderBy(x => x.OpportunityValue),
            _ => query.SortDescending
                ? opportunities.OrderByDescending(x => x.CreatedAt)
                : opportunities.OrderBy(x => x.CreatedAt)
        };

        var total = await opportunities.LongCountAsync(cancellationToken);
        var items = await opportunities
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var leadIds = items.Where(x => x.LeadId.HasValue).Select(x => x.LeadId!.Value).Distinct().ToList();
        var leadNumbers = await _db.Leads.AsNoTracking()
            .Where(x => leadIds.Contains(x.Id))
            .Select(x => new { x.Id, x.LeadNumber })
            .ToDictionaryAsync(x => x.Id, x => x.LeadNumber, cancellationToken);

        var data = items.Select(o =>
        {
            var dto = _mapper.Map<OpportunityDto>(o);
            if (o.LeadId.HasValue && leadNumbers.TryGetValue(o.LeadId.Value, out var leadNumber))
            {
                dto.LeadNumber = leadNumber;
            }

            return dto;
        }).ToList();

        return PagedResponse<OpportunityDto>.Create(data, page, pageSize, total);
    }
}

public sealed record GetOpportunityTimelineQuery(Guid OpportunityId)
    : IRequest<IReadOnlyList<OpportunityFollowUpDto>>;

public sealed class GetOpportunityTimelineQueryHandler
    : IRequestHandler<GetOpportunityTimelineQuery, IReadOnlyList<OpportunityFollowUpDto>>
{
    private readonly CrmDbContext _db;
    private readonly IMapper _mapper;

    public GetOpportunityTimelineQueryHandler(CrmDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<OpportunityFollowUpDto>> Handle(
        GetOpportunityTimelineQuery query,
        CancellationToken cancellationToken)
    {
        var exists = await _db.Opportunities.AnyAsync(x => x.Id == query.OpportunityId, cancellationToken);
        if (!exists)
        {
            throw new NotFoundException($"Opportunity '{query.OpportunityId}' was not found.");
        }

        var followUps = await _db.OpportunityFollowUps.AsNoTracking()
            .Where(x => x.OpportunityId == query.OpportunityId)
            .OrderByDescending(x => x.FollowUpDate)
            .ThenByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<OpportunityFollowUpDto>>(followUps);
    }
}

public sealed record GetLeadHistoryQuery(Guid LeadId) : IRequest<IReadOnlyList<CrmHistoryItemDto>>;

public sealed class GetLeadHistoryQueryHandler : IRequestHandler<GetLeadHistoryQuery, IReadOnlyList<CrmHistoryItemDto>>
{
    private readonly CrmDbContext _db;

    public GetLeadHistoryQueryHandler(CrmDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<CrmHistoryItemDto>> Handle(
        GetLeadHistoryQuery query,
        CancellationToken cancellationToken)
    {
        var lead = await _db.Leads.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == query.LeadId, cancellationToken)
            ?? throw new NotFoundException($"Lead '{query.LeadId}' was not found.");

        var leadFollowUps = await _db.LeadFollowUps.AsNoTracking()
            .Where(x => x.LeadId == lead.Id)
            .Select(x => new CrmHistoryItemDto
            {
                Id = x.Id,
                Source = "Lead",
                ParentId = lead.Id,
                ParentNumber = lead.LeadNumber,
                ActivityType = x.ActivityType,
                FollowUpDate = x.FollowUpDate,
                NextFollowUpDate = x.NextFollowUpDate,
                Remarks = x.Remarks,
                Status = x.Status,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy
            })
            .ToListAsync(cancellationToken);

        var opportunityFollowUps = new List<CrmHistoryItemDto>();
        if (lead.ConvertedOpportunityId.HasValue)
        {
            var opportunity = await _db.Opportunities.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == lead.ConvertedOpportunityId.Value, cancellationToken);

            if (opportunity is not null)
            {
                opportunityFollowUps = await _db.OpportunityFollowUps.AsNoTracking()
                    .Where(x => x.OpportunityId == opportunity.Id)
                    .Select(x => new CrmHistoryItemDto
                    {
                        Id = x.Id,
                        Source = "Opportunity",
                        ParentId = opportunity.Id,
                        ParentNumber = opportunity.OpportunityNumber,
                        ActivityType = x.ActivityType,
                        FollowUpDate = x.FollowUpDate,
                        NextFollowUpDate = x.NextFollowUpDate,
                        Remarks = x.Remarks,
                        Status = x.Status,
                        CreatedAt = x.CreatedAt,
                        CreatedBy = x.CreatedBy
                    })
                    .ToListAsync(cancellationToken);
            }
        }

        return leadFollowUps
            .Concat(opportunityFollowUps)
            .OrderByDescending(x => x.FollowUpDate)
            .ThenByDescending(x => x.CreatedAt)
            .ToList();
    }
}
