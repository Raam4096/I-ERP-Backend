using iERP.Modules.Platform.DynamicModules.Application;
using iERP.Modules.Platform.DynamicModules.Application.Dtos;
using iERP.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace iERP.Modules.Platform.DynamicModules.Api;

public static class DynamicModulesEndpoints
{
    public static IEndpointRouteBuilder MapDynamicModulesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/dynamic_modules")
            .WithTags("DynamicModules");

        group.MapGet("/health", () => Results.Ok(ApiResponse<string>.Ok("DynamicModules module ready")))
            .WithName("DynamicModulesHealth")
            .AllowAnonymous();

        // Modules (navbar / Screen Architect root)
        group.MapGet("/", ListModulesAsync)
            .WithName("ListDynamicModules")
            .RequireAuthorization()
            .Produces<ApiResponse<IReadOnlyList<DynamicModuleDto>>>(StatusCodes.Status200OK);

        group.MapPost("/", CreateModuleAsync)
            .WithName("CreateDynamicModule")
            .RequireAuthorization()
            .Produces<ApiResponse<DynamicModuleDto>>(StatusCodes.Status201Created)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapGet("/{moduleId:guid}", GetModuleAsync)
            .WithName("GetDynamicModule")
            .RequireAuthorization()
            .Produces<ApiResponse<DynamicModuleDto>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        group.MapPut("/{moduleId:guid}", UpdateModuleAsync)
            .WithName("UpdateDynamicModule")
            .RequireAuthorization()
            .Produces<ApiResponse<DynamicModuleDto>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        group.MapDelete("/{moduleId:guid}", DeleteModuleAsync)
            .WithName("DeleteDynamicModule")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        // Entities (screens under a module)
        group.MapPost("/{moduleId:guid}/entities", CreateEntityAsync)
            .WithName("CreateDynamicEntity")
            .RequireAuthorization()
            .Produces<ApiResponse<DynamicEntityDto>>(StatusCodes.Status201Created)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        group.MapGet("/entities/{entityId:guid}", GetEntityAsync)
            .WithName("GetDynamicEntity")
            .RequireAuthorization()
            .Produces<ApiResponse<DynamicEntityDto>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        group.MapPut("/entities/{entityId:guid}", UpdateEntityAsync)
            .WithName("UpdateDynamicEntity")
            .RequireAuthorization()
            .Produces<ApiResponse<DynamicEntityDto>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        group.MapDelete("/entities/{entityId:guid}", DeleteEntityAsync)
            .WithName("DeleteDynamicEntity")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        // Fields
        group.MapPost("/entities/{entityId:guid}/fields", CreateFieldAsync)
            .WithName("CreateDynamicField")
            .RequireAuthorization()
            .Produces<ApiResponse<DynamicFieldDto>>(StatusCodes.Status201Created)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        group.MapPut("/fields/{fieldId:guid}", UpdateFieldAsync)
            .WithName("UpdateDynamicField")
            .RequireAuthorization()
            .Produces<ApiResponse<DynamicFieldDto>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        group.MapDelete("/fields/{fieldId:guid}", DeleteFieldAsync)
            .WithName("DeleteDynamicField")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        // Records (runtime data)
        group.MapGet("/entities/{entityId:guid}/records", ListRecordsAsync)
            .WithName("ListDynamicRecords")
            .RequireAuthorization()
            .Produces<ApiResponse<IReadOnlyList<DynamicRecordDto>>>(StatusCodes.Status200OK);

        group.MapPost("/entities/{entityId:guid}/records", CreateRecordAsync)
            .WithName("CreateDynamicRecord")
            .RequireAuthorization()
            .Produces<ApiResponse<DynamicRecordDto>>(StatusCodes.Status201Created)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapGet("/records/{recordId:guid}", GetRecordAsync)
            .WithName("GetDynamicRecord")
            .RequireAuthorization()
            .Produces<ApiResponse<DynamicRecordDto>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        group.MapPut("/records/{recordId:guid}", UpdateRecordAsync)
            .WithName("UpdateDynamicRecord")
            .RequireAuthorization()
            .Produces<ApiResponse<DynamicRecordDto>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        group.MapDelete("/records/{recordId:guid}", DeleteRecordAsync)
            .WithName("DeleteDynamicRecord")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<IResult> ListModulesAsync(
        IDynamicModulesService service,
        bool? activeOnly,
        CancellationToken cancellationToken)
    {
        var modules = await service.ListModulesAsync(activeOnly ?? true, cancellationToken);
        return Results.Ok(ApiResponse<IReadOnlyList<DynamicModuleDto>>.Ok(modules));
    }

    private static async Task<IResult> CreateModuleAsync(
        CreateDynamicModuleRequest request,
        IDynamicModulesService service,
        CancellationToken cancellationToken)
    {
        var module = await service.CreateModuleAsync(request, cancellationToken);
        return Results.Created($"/api/v1/dynamic_modules/{module.Id}", ApiResponse<DynamicModuleDto>.Ok(module, "Module created."));
    }

    private static async Task<IResult> GetModuleAsync(
        Guid moduleId,
        IDynamicModulesService service,
        CancellationToken cancellationToken)
    {
        var module = await service.GetModuleAsync(moduleId, cancellationToken);
        return Results.Ok(ApiResponse<DynamicModuleDto>.Ok(module));
    }

    private static async Task<IResult> UpdateModuleAsync(
        Guid moduleId,
        UpdateDynamicModuleRequest request,
        IDynamicModulesService service,
        CancellationToken cancellationToken)
    {
        var module = await service.UpdateModuleAsync(moduleId, request, cancellationToken);
        return Results.Ok(ApiResponse<DynamicModuleDto>.Ok(module, "Module updated."));
    }

    private static async Task<IResult> DeleteModuleAsync(
        Guid moduleId,
        IDynamicModulesService service,
        CancellationToken cancellationToken)
    {
        await service.DeleteModuleAsync(moduleId, cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> CreateEntityAsync(
        Guid moduleId,
        CreateDynamicEntityRequest request,
        IDynamicModulesService service,
        CancellationToken cancellationToken)
    {
        var entity = await service.CreateEntityAsync(moduleId, request, cancellationToken);
        return Results.Created($"/api/v1/dynamic_modules/entities/{entity.Id}", ApiResponse<DynamicEntityDto>.Ok(entity, "Entity created."));
    }

    private static async Task<IResult> GetEntityAsync(
        Guid entityId,
        IDynamicModulesService service,
        CancellationToken cancellationToken)
    {
        var entity = await service.GetEntityAsync(entityId, cancellationToken);
        return Results.Ok(ApiResponse<DynamicEntityDto>.Ok(entity));
    }

    private static async Task<IResult> UpdateEntityAsync(
        Guid entityId,
        UpdateDynamicEntityRequest request,
        IDynamicModulesService service,
        CancellationToken cancellationToken)
    {
        var entity = await service.UpdateEntityAsync(entityId, request, cancellationToken);
        return Results.Ok(ApiResponse<DynamicEntityDto>.Ok(entity, "Entity updated."));
    }

    private static async Task<IResult> DeleteEntityAsync(
        Guid entityId,
        IDynamicModulesService service,
        CancellationToken cancellationToken)
    {
        await service.DeleteEntityAsync(entityId, cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> CreateFieldAsync(
        Guid entityId,
        CreateDynamicFieldRequest request,
        IDynamicModulesService service,
        CancellationToken cancellationToken)
    {
        var field = await service.CreateFieldAsync(entityId, request, cancellationToken);
        return Results.Created($"/api/v1/dynamic_modules/fields/{field.Id}", ApiResponse<DynamicFieldDto>.Ok(field, "Field created."));
    }

    private static async Task<IResult> UpdateFieldAsync(
        Guid fieldId,
        UpdateDynamicFieldRequest request,
        IDynamicModulesService service,
        CancellationToken cancellationToken)
    {
        var field = await service.UpdateFieldAsync(fieldId, request, cancellationToken);
        return Results.Ok(ApiResponse<DynamicFieldDto>.Ok(field, "Field updated."));
    }

    private static async Task<IResult> DeleteFieldAsync(
        Guid fieldId,
        IDynamicModulesService service,
        CancellationToken cancellationToken)
    {
        await service.DeleteFieldAsync(fieldId, cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> ListRecordsAsync(
        Guid entityId,
        IDynamicModulesService service,
        CancellationToken cancellationToken)
    {
        var records = await service.ListRecordsAsync(entityId, cancellationToken);
        return Results.Ok(ApiResponse<IReadOnlyList<DynamicRecordDto>>.Ok(records));
    }

    private static async Task<IResult> CreateRecordAsync(
        Guid entityId,
        UpsertDynamicRecordRequest request,
        IDynamicModulesService service,
        CancellationToken cancellationToken)
    {
        var record = await service.CreateRecordAsync(entityId, request, cancellationToken);
        return Results.Created($"/api/v1/dynamic_modules/records/{record.Id}", ApiResponse<DynamicRecordDto>.Ok(record, "Record created."));
    }

    private static async Task<IResult> GetRecordAsync(
        Guid recordId,
        IDynamicModulesService service,
        CancellationToken cancellationToken)
    {
        var record = await service.GetRecordAsync(recordId, cancellationToken);
        return Results.Ok(ApiResponse<DynamicRecordDto>.Ok(record));
    }

    private static async Task<IResult> UpdateRecordAsync(
        Guid recordId,
        UpsertDynamicRecordRequest request,
        IDynamicModulesService service,
        CancellationToken cancellationToken)
    {
        var record = await service.UpdateRecordAsync(recordId, request, cancellationToken);
        return Results.Ok(ApiResponse<DynamicRecordDto>.Ok(record, "Record updated."));
    }

    private static async Task<IResult> DeleteRecordAsync(
        Guid recordId,
        IDynamicModulesService service,
        CancellationToken cancellationToken)
    {
        await service.DeleteRecordAsync(recordId, cancellationToken);
        return Results.NoContent();
    }
}
