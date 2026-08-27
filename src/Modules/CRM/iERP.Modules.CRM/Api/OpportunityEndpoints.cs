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

public static class OpportunityEndpoints
{
    public static IEndpointRouteBuilder MapOpportunityEndpoints(this IEndpointRouteBuilder app)
    {
        MapOpportunityGroup(
            app,
            "/api/crm/opportunities",
            "/api/crm/opportunity-followups",
            "CRM Opportunities",
            "CRM Opportunity FollowUps",
            string.Empty);

        MapOpportunityGroup(
            app,
            "/api/v1/crm/opportunities",
            "/api/v1/crm/opportunity-followups",
            "CRM Opportunities v1",
            "CRM Opportunity FollowUps v1",
            "V1");

        return app;
    }

    private static void MapOpportunityGroup(
        IEndpointRouteBuilder app,
        string opportunitiesPrefix,
        string followUpsPrefix,
        string opportunitiesTag,
        string followUpsTag,
        string nameSuffix)
    {
        var opportunities = app.MapGroup(opportunitiesPrefix)
            .WithTags(opportunitiesTag)
            .RequireAuthorization();

        opportunities.MapGet("/", GetOpportunitiesAsync)
            .WithName("GetOpportunities" + nameSuffix)
            .Produces<PagedResponse<OpportunityDto>>(StatusCodes.Status200OK);

        opportunities.MapGet("/{id:guid}", GetOpportunityByIdAsync)
            .WithName("GetOpportunityById" + nameSuffix)
            .Produces<ApiResponse<OpportunityDto>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        opportunities.MapPut("/{id:guid}", UpdateOpportunityAsync)
            .WithName("UpdateOpportunity" + nameSuffix)
            .Produces<ApiResponse<OpportunityDto>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ApiErrorResponse>(StatusCodes.Status409Conflict);

        opportunities.MapPost("/{id:guid}/discard", DiscardOpportunityAsync)
            .WithName("DiscardOpportunity" + nameSuffix)
            .Produces<ApiResponse<OpportunityDto>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ApiErrorResponse>(StatusCodes.Status409Conflict);

        opportunities.MapPost("/{id:guid}/restore", RestoreOpportunityAsync)
            .WithName("RestoreOpportunity" + nameSuffix)
            .Produces<ApiResponse<OpportunityDto>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ApiErrorResponse>(StatusCodes.Status409Conflict);

        opportunities.MapDelete("/{id:guid}", DeleteOpportunityAsync)
            .WithName("DeleteOpportunity" + nameSuffix)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        opportunities.MapPost("/{opportunityId:guid}/followups", CreateFollowUpAsync)
            .WithName("CreateOpportunityFollowUp" + nameSuffix)
            .Produces<ApiResponse<OpportunityFollowUpDto>>(StatusCodes.Status201Created)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        opportunities.MapGet("/{opportunityId:guid}/timeline", GetTimelineAsync)
            .WithName("GetOpportunityTimeline" + nameSuffix)
            .Produces<ApiResponse<IReadOnlyList<OpportunityFollowUpDto>>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        var followUps = app.MapGroup(followUpsPrefix)
            .WithTags(followUpsTag)
            .RequireAuthorization();

        followUps.MapPut("/{id:guid}", UpdateFollowUpAsync)
            .WithName("UpdateOpportunityFollowUp" + nameSuffix)
            .Produces<ApiResponse<OpportunityFollowUpDto>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> GetOpportunitiesAsync(
        IMediator mediator,
        CancellationToken cancellationToken,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? status = null,
        [FromQuery] Guid? leadId = null,
        [FromQuery] Guid? ownerUserId = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool? sortDescending = null)
    {
        var result = await mediator.Send(
            new GetOpportunitiesQuery(
                page <= 0 ? 1 : page,
                pageSize <= 0 ? PaginationDefaults.DefaultPageSize : pageSize,
                search,
                status,
                leadId,
                ownerUserId,
                sortBy,
                sortDescending ?? true),
            cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetOpportunityByIdAsync(
        Guid id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetOpportunityByIdQuery(id), cancellationToken);
        return Results.Ok(ApiResponse<OpportunityDto>.Ok(result));
    }

    private static async Task<IResult> UpdateOpportunityAsync(
        Guid id,
        [FromBody] UpdateOpportunityRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateOpportunityCommand(id, request), cancellationToken);
        return Results.Ok(ApiResponse<OpportunityDto>.Ok(result, "Opportunity updated successfully."));
    }

    private static async Task<IResult> DiscardOpportunityAsync(
        Guid id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DiscardOpportunityCommand(id), cancellationToken);
        return Results.Ok(ApiResponse<OpportunityDto>.Ok(result, "Opportunity discarded."));
    }

    private static async Task<IResult> RestoreOpportunityAsync(
        Guid id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RestoreOpportunityCommand(id), cancellationToken);
        return Results.Ok(ApiResponse<OpportunityDto>.Ok(result, "Opportunity restored."));
    }

    private static async Task<IResult> DeleteOpportunityAsync(
        Guid id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteOpportunityCommand(id), cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> CreateFollowUpAsync(
        Guid opportunityId,
        [FromBody] CreateOpportunityFollowUpRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new CreateOpportunityFollowUpCommand(opportunityId, request),
            cancellationToken);
        return Results.Created(
            $"/api/crm/opportunity-followups/{result.Id}",
            ApiResponse<OpportunityFollowUpDto>.Ok(result, "Follow-up created successfully."));
    }

    private static async Task<IResult> GetTimelineAsync(
        Guid opportunityId,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetOpportunityTimelineQuery(opportunityId), cancellationToken);
        return Results.Ok(ApiResponse<IReadOnlyList<OpportunityFollowUpDto>>.Ok(result));
    }

    private static async Task<IResult> UpdateFollowUpAsync(
        Guid id,
        [FromBody] UpdateOpportunityFollowUpRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateOpportunityFollowUpCommand(id, request), cancellationToken);
        return Results.Ok(ApiResponse<OpportunityFollowUpDto>.Ok(result, "Follow-up updated successfully."));
    }
}
