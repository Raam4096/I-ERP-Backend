using iERP.Modules.Platform.Metadata.Application;
using iERP.Modules.Platform.Metadata.Application.Dtos;
using iERP.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace iERP.Modules.Platform.Metadata.Api;

public static class MetadataEndpoints
{
    public static IEndpointRouteBuilder MapMetadataEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/metadata")
            .WithTags("Metadata");

        group.MapGet("/health", () => Results.Ok(ApiResponse<string>.Ok("Metadata module ready")))
            .WithName("MetadataHealth")
            .AllowAnonymous();

        group.MapGet("/modules", ListModulesAsync)
            .WithName("ListMetadataModules")
            .RequireAuthorization()
            .Produces<ApiResponse<IReadOnlyList<MetadataModuleDto>>>(StatusCodes.Status200OK);

        group.MapGet("/screens/{screenCode}", GetScreenAsync)
            .WithName("GetMetadataScreen")
            .RequireAuthorization()
            .Produces<ApiResponse<GenericPageDto>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        group.MapPut("/screens/{screenCode}/preferences", SavePreferencesAsync)
            .WithName("SaveScreenFieldPreferences")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapGet("/entities/{entityName}/custom-fields", ListCustomFieldsAsync)
            .WithName("ListCustomFieldDefinitions")
            .RequireAuthorization()
            .Produces<ApiResponse<IReadOnlyList<CustomFieldDefinitionDto>>>(StatusCodes.Status200OK);

        group.MapPost("/entities/{entityName}/custom-fields", CreateCustomFieldAsync)
            .WithName("CreateCustomFieldDefinition")
            .RequireAuthorization()
            .Produces<ApiResponse<CustomFieldDefinitionDto>>(StatusCodes.Status201Created)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapPut("/custom-fields/{id:guid}", UpdateCustomFieldAsync)
            .WithName("UpdateCustomFieldDefinition")
            .RequireAuthorization()
            .Produces<ApiResponse<CustomFieldDefinitionDto>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        group.MapDelete("/custom-fields/{id:guid}", DeleteCustomFieldAsync)
            .WithName("DeleteCustomFieldDefinition")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<IResult> ListModulesAsync(
        IMetadataCatalogService catalogService,
        bool? activeOnly,
        CancellationToken cancellationToken)
    {
        var modules = await catalogService.ListModulesAsync(activeOnly ?? true, cancellationToken);
        return Results.Ok(ApiResponse<IReadOnlyList<MetadataModuleDto>>.Ok(modules));
    }

    private static async Task<IResult> GetScreenAsync(
        string screenCode,
        IMetadataScreenService screenService,
        CancellationToken cancellationToken)
    {
        var page = await screenService.GetScreenAsync(screenCode, cancellationToken);
        return Results.Ok(ApiResponse<GenericPageDto>.Ok(page));
    }

    private static async Task<IResult> SavePreferencesAsync(
        string screenCode,
        SaveScreenFieldPreferencesRequest request,
        IUserFieldPreferenceService preferenceService,
        CancellationToken cancellationToken)
    {
        await preferenceService.SavePreferencesAsync(screenCode, request, cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> ListCustomFieldsAsync(
        string entityName,
        ICustomFieldDefinitionService customFieldService,
        CancellationToken cancellationToken)
    {
        var fields = await customFieldService.ListAsync(entityName, cancellationToken);
        return Results.Ok(ApiResponse<IReadOnlyList<CustomFieldDefinitionDto>>.Ok(fields));
    }

    private static async Task<IResult> CreateCustomFieldAsync(
        string entityName,
        CreateCustomFieldDefinitionRequest request,
        ICustomFieldDefinitionService customFieldService,
        CancellationToken cancellationToken)
    {
        var field = await customFieldService.CreateAsync(entityName, request, cancellationToken);
        return Results.Created(
            $"/api/v1/metadata/custom-fields/{field.Id}",
            ApiResponse<CustomFieldDefinitionDto>.Ok(field, "Custom field created."));
    }

    private static async Task<IResult> UpdateCustomFieldAsync(
        Guid id,
        UpdateCustomFieldDefinitionRequest request,
        ICustomFieldDefinitionService customFieldService,
        CancellationToken cancellationToken)
    {
        var field = await customFieldService.UpdateAsync(id, request, cancellationToken);
        return Results.Ok(ApiResponse<CustomFieldDefinitionDto>.Ok(field, "Custom field updated."));
    }

    private static async Task<IResult> DeleteCustomFieldAsync(
        Guid id,
        ICustomFieldDefinitionService customFieldService,
        CancellationToken cancellationToken)
    {
        await customFieldService.DeleteAsync(id, cancellationToken);
        return Results.NoContent();
    }
}
