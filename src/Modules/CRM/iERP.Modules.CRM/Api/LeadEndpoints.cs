using iERP.Modules.CRM.Application.Leads.Commands;
using iERP.Modules.CRM.Application.Leads.Dtos;
using iERP.Modules.CRM.Application.Leads.Queries;
using iERP.Modules.CRM.Application.Opportunities.Commands;
using iERP.Modules.CRM.Application.Opportunities.Dtos;
using iERP.Modules.CRM.Application.Opportunities.Queries;
using iERP.SharedKernel.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace iERP.Modules.CRM.Api;

public static class LeadEndpoints
{
    public static IEndpointRouteBuilder MapLeadEndpoints(this IEndpointRouteBuilder app)
    {
        MapLeadGroup(app, "/api/v1/crm/leads", "/api/v1/crm/followups", "CRM Leads", "CRM FollowUps", string.Empty);
        return app;
    }

    private static void MapLeadGroup(
        IEndpointRouteBuilder app,
        string leadsPrefix,
        string followUpsPrefix,
        string leadsTag,
        string followUpsTag,
        string nameSuffix)
    {
        var leads = app.MapGroup(leadsPrefix)
            .WithTags(leadsTag)
            .RequireAuthorization();

        leads.MapPost("/", CreateLeadAsync)
            .WithName("CreateLead" + nameSuffix)
            .Produces<ApiResponse<LeadDto>>(StatusCodes.Status201Created)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiErrorResponse>(StatusCodes.Status409Conflict);

        leads.MapGet("/", GetLeadsAsync)
            .WithName("GetLeads" + nameSuffix)
            .Produces<PagedResponse<LeadDto>>(StatusCodes.Status200OK);

        leads.MapGet("/{id:guid}", GetLeadByIdAsync)
            .WithName("GetLeadById" + nameSuffix)
            .Produces<ApiResponse<LeadDto>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        leads.MapGet("/{id:guid}/form", GetLeadFormDataAsync)
            .WithName("GetLeadFormData" + nameSuffix)
            .Produces<ApiResponse<LeadFormDataDto>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        leads.MapPut("/{id:guid}", UpdateLeadAsync)
            .WithName("UpdateLead" + nameSuffix)
            .Produces<ApiResponse<LeadDto>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ApiErrorResponse>(StatusCodes.Status409Conflict);

        leads.MapDelete("/{id:guid}", DeleteLeadAsync)
            .WithName("DeleteLead" + nameSuffix)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        leads.MapPost("/{leadId:guid}/convert-to-opportunity", ConvertToOpportunityAsync)
            .WithName("ConvertLeadToOpportunity" + nameSuffix)
            .Produces<ApiResponse<OpportunityDto>>(StatusCodes.Status201Created)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ApiErrorResponse>(StatusCodes.Status409Conflict);

        leads.MapPost("/{leadId:guid}/followups", CreateFollowUpAsync)
            .WithName("CreateLeadFollowUp" + nameSuffix)
            .Produces<ApiResponse<LeadFollowUpDto>>(StatusCodes.Status201Created)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        leads.MapGet("/{leadId:guid}/timeline", GetTimelineAsync)
            .WithName("GetLeadTimeline" + nameSuffix)
            .Produces<ApiResponse<IReadOnlyList<LeadFollowUpDto>>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        leads.MapGet("/{leadId:guid}/history", GetLeadHistoryAsync)
            .WithName("GetLeadHistory" + nameSuffix)
            .Produces<ApiResponse<IReadOnlyList<CrmHistoryItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        var followUps = app.MapGroup(followUpsPrefix)
            .WithTags(followUpsTag)
            .RequireAuthorization();

        followUps.MapPut("/{id:guid}", UpdateFollowUpAsync)
            .WithName("UpdateFollowUp" + nameSuffix)
            .Produces<ApiResponse<LeadFollowUpDto>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> CreateLeadAsync(
        [FromBody] CreateLeadRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateLeadCommand(request), cancellationToken);
        return Results.Created($"/api/v1/crm/leads/{result.Id}", ApiResponse<LeadDto>.Ok(result, "Lead created successfully."));
    }

    private static async Task<IResult> GetLeadsAsync(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] Guid? assignedToUserId,
        [FromQuery] string? sortBy,
        [FromQuery] bool? sortDescending,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetLeadsQuery(
                page <= 0 ? 1 : page,
                pageSize <= 0 ? PaginationDefaults.DefaultPageSize : pageSize,
                search,
                status,
                assignedToUserId,
                sortBy,
                sortDescending ?? true),
            cancellationToken);

        return Results.Ok(result);
    }

    private static async Task<IResult> GetLeadByIdAsync(
        Guid id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetLeadByIdQuery(id), cancellationToken);
        return Results.Ok(ApiResponse<LeadDto>.Ok(result));
    }

    private static async Task<IResult> GetLeadFormDataAsync(
        Guid id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetLeadFormDataQuery(id), cancellationToken);
        return Results.Ok(ApiResponse<LeadFormDataDto>.Ok(result));
    }

    private static async Task<IResult> UpdateLeadAsync(
        Guid id,
        [FromBody] UpdateLeadRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateLeadCommand(id, request), cancellationToken);
        return Results.Ok(ApiResponse<LeadDto>.Ok(result, "Lead updated successfully."));
    }

    private static async Task<IResult> DeleteLeadAsync(
        Guid id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteLeadCommand(id), cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> ConvertToOpportunityAsync(
        Guid leadId,
        [FromBody] ConvertLeadToOpportunityRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ConvertLeadToOpportunityCommand(leadId, request), cancellationToken);
        return Results.Created(
            $"/api/v1/crm/opportunities/{result.Id}",
            ApiResponse<OpportunityDto>.Ok(result, "Lead converted to opportunity successfully."));
    }

    private static async Task<IResult> CreateFollowUpAsync(
        Guid leadId,
        [FromBody] CreateFollowUpRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateFollowUpCommand(leadId, request), cancellationToken);
        return Results.Created(
            $"/api/v1/crm/followups/{result.Id}",
            ApiResponse<LeadFollowUpDto>.Ok(result, "Follow-up created successfully."));
    }

    private static async Task<IResult> UpdateFollowUpAsync(
        Guid id,
        [FromBody] UpdateFollowUpRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateFollowUpCommand(id, request), cancellationToken);
        return Results.Ok(ApiResponse<LeadFollowUpDto>.Ok(result, "Follow-up updated successfully."));
    }

    private static async Task<IResult> GetTimelineAsync(
        Guid leadId,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetLeadTimelineQuery(leadId), cancellationToken);
        return Results.Ok(ApiResponse<IReadOnlyList<LeadFollowUpDto>>.Ok(result));
    }

    private static async Task<IResult> GetLeadHistoryAsync(
        Guid leadId,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetLeadHistoryQuery(leadId), cancellationToken);
        return Results.Ok(ApiResponse<IReadOnlyList<CrmHistoryItemDto>>.Ok(result));
    }
}
