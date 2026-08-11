using iERP.Modules.Platform.Attachments.Api;
using iERP.Modules.Platform.Audit.Api;
using iERP.Modules.Platform.DynamicModules.Api;
using iERP.Modules.Platform.Identity.Api;
using iERP.Modules.Platform.Metadata.Api;
using iERP.Modules.Platform.Notifications.Api;
using iERP.Modules.Platform.Organization.Api;
using iERP.Modules.Platform.Settings.Api;
using iERP.Modules.Platform.Tenancy.Api;
using Microsoft.AspNetCore.Routing;

namespace iERP.Modules.Platform.Api;

public static class PlatformModuleEndpoints
{
    public static IEndpointRouteBuilder MapPlatformEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapAuthEndpoints();
        app.MapTenantsEndpoints();
        app.MapOrganizationEndpoints();
        app.MapMetadataEndpoints();
        app.MapSettingsEndpoints();
        app.MapAuditEndpoints();
        app.MapAttachmentsEndpoints();
        app.MapNotificationsEndpoints();
        app.MapDynamicModulesEndpoints();
        return app;
    }
}
