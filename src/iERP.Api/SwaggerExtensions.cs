using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;

namespace iERP.Api;

public sealed class IerpSwaggerOptions
{
    public const string SectionName = "Swagger";

    public string[] VisibleTags { get; init; } =
    [
        "CRM Leads",
        "CRM FollowUps"
    ];
}

public static class SwaggerExtensions
{
    public static IServiceCollection AddIerpSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        var swaggerOptions = configuration.GetSection(IerpSwaggerOptions.SectionName).Get<IerpSwaggerOptions>() ?? new IerpSwaggerOptions();
        var visibleTags = swaggerOptions.VisibleTags
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "i-ERP API", Version = "v1" });
            options.DocInclusionPredicate((_, apiDescription) => ShouldIncludeEndpoint(apiDescription, visibleTags));
        });

        return services;
    }

    private static bool ShouldIncludeEndpoint(ApiDescription apiDescription, IReadOnlySet<string> visibleTags)
    {
        if (visibleTags.Count == 0)
        {
            return false;
        }

        return apiDescription.ActionDescriptor.EndpointMetadata
            .OfType<ITagsMetadata>()
            .SelectMany(metadata => metadata.Tags)
            .Any(visibleTags.Contains);
    }
}
