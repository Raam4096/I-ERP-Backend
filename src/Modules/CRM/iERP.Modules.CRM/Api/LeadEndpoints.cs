using iERP.Modules.CRM.Application.Leads.Commands;
using iERP.Modules.CRM.Application.Leads.Dtos;
using iERP.Modules.CRM.Application.Leads.Queries;
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
        var leads = app.MapGroup("/api/crm/leads")
            .WithTags("CRM Leads")
            .RequireAuthorization();

        leads.MapPost("/", CreateLeadAsync)
            .WithName("CreateLead")
            .Produces<ApiResponse<LeadDto>>(StatusCodes.Status201Created)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiErrorResponse>(StatusCodes.Status409Conflict);

        leads.MapGet("/", GetLeadsAsync)
            .WithName("GetLeads")
            .Produces<PagedResponse<LeadDto>>(StatusCodes.Status200OK);

        leads.MapGet("/{id:guid}", GetLeadByIdAsync)
            .WithName("GetLeadById")
            .Produces<ApiResponse<LeadDto>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        leads.MapPut("/{id:guid}", UpdateLeadAsync)
            .WithName("UpdateLead")
            .Produces<ApiResponse<LeadDto>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ApiErrorResponse>(StatusCodes.Status409Conflict);

        leads.MapDelete("/{id:guid}", DeleteLeadAsync)
            .WithName("DeleteLead")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        leads.MapPost("/{leadId:guid}/followups", CreateFollowUpAsync)
            .WithName("CreateLeadFollowUp")
            .Produces<ApiResponse<LeadFollowUpDto>>(StatusCodes.Status201Created)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        leads.MapGet("/{leadId:guid}/timeline", GetTimelineAsync)
            .WithName("GetLeadTimeline")
            .Produces<ApiResponse<IReadOnlyList<LeadFollowUpDto>>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        var followUps = app.MapGroup("/api/crm/followups")
            .WithTags("CRM FollowUps")
            .RequireAuthorization();

        followUps.MapPut("/{id:guid}", UpdateFollowUpAsync)
            .WithName("UpdateFollowUp")
            .Produces<ApiResponse<LeadFollowUpDto>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<IResult> CreateLeadAsync(
        [FromBody] CreateLeadRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateLeadCommand(request), cancellationToken);
        return Results.Created($"/api/crm/leads/{result.Id}", ApiResponse<LeadDto>.Ok(result, "Lead created successfully."));
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

    private static async Task<IResult> CreateFollowUpAsync(
        Guid leadId,
        [FromBody] CreateFollowUpRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateFollowUpCommand(leadId, request), cancellationToken);
        return Results.Created(
            $"/api/crm/followups/{result.Id}",
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
}
