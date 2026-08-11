#!/usr/bin/env python3
"""Generate EF configurations, DbContexts, DI, endpoints, API/Worker hosts, migrations helpers, tests, docs."""
from __future__ import annotations

from pathlib import Path
from textwrap import dedent

ROOT = Path(__file__).resolve().parents[1]


def write(rel: str, content: str) -> None:
    path = ROOT / rel
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(dedent(content).lstrip("\n").replace("\r\n", "\n"), encoding="utf-8")
    print(rel)


def simple_config(ns: str, entity: str, schema: str, table: str, extras: str = "", base: str = "AuditableEntity") -> str:
    if base == "AuditableEntity":
        return f"""
using iERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace {ns};

public sealed class {entity}Configuration : AuditableEntityConfiguration<{entity}>
{{
    public override void Configure(EntityTypeBuilder<{entity}> builder)
    {{
        base.Configure(builder);
        builder.ToTable("{table}", "{schema}");
{extras}
    }}
}}
"""
    return f"""
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace {ns};

public sealed class {entity}Configuration : IEntityTypeConfiguration<{entity}>
{{
    public void Configure(EntityTypeBuilder<{entity}> builder)
    {{
        builder.ToTable("{table}", "{schema}");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
{extras}
    }}
}}
"""


def dbcontext(ns: str, name: str, schema: str, entities: list[str], usings: str) -> str:
    sets = "\n".join(f"    public DbSet<{e}> {e}s => Set<{e}>();" for e in entities)
    # Fix pluralization quirks for known names
    sets = sets.replace("Addresss", "Addresses")
    sets = sets.replace("Activitys", "Activities")
    sets = sets.replace("Opportunitys", "Opportunities")
    sets = sets.replace("Categorys", "Categories")
    sets = sets.replace("Historys", "Histories")
    sets = sets.replace("Currencys", "Currencies")
    sets = sets.replace("Subsidiarys", "Subsidiaries")
    return f"""
using iERP.Infrastructure.Persistence;
using iERP.Infrastructure.Persistence.Interceptors;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;
{usings}

namespace {ns};

public sealed class {name} : DbContext
{{
    private readonly ITenantContext _tenantContext;

    public {name}(DbContextOptions<{name}> options, ITenantContext tenantContext) : base(options)
    {{
        _tenantContext = tenantContext;
    }}

{sets}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {{
        modelBuilder.HasDefaultSchema("{schema}");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof({name}).Assembly);
        modelBuilder.ApplySnakeCaseNamingConvention();
        modelBuilder.ConfigureMoneyPrecision();
        modelBuilder.ApplyTenantAndSoftDeleteFilters(_tenantContext);
        base.OnModelCreating(modelBuilder);
    }}
}}
"""


def endpoint(ns: str, module: str, route: str) -> str:
    return f"""
using iERP.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace {ns};

public static class {module}Endpoints
{{
    public static IEndpointRouteBuilder Map{module}Endpoints(this IEndpointRouteBuilder app)
    {{
        var group = app.MapGroup("/api/v1/{route}").WithTags("{module}");
        group.MapGet("/health", () => Results.Ok(ApiResponse<string>.Ok("{module} module ready")))
            .WithName("{module}Health");
        return app;
    }}
}}
"""


# ================= PLATFORM CONFIGS =================
P = "src/Modules/Platform/iERP.Modules.Platform"

write(f"{P}/Tenancy/Infrastructure/Configurations/TenantConfiguration.cs", """
using iERP.Modules.Platform.Tenancy.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Platform.Tenancy.Infrastructure.Configurations;

public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("tenants", "platform");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Code).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(256).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(32).IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
""")

write(f"{P}/Tenancy/Infrastructure/Configurations/OutboxMessageConfiguration.cs", """
using iERP.Modules.Platform.Tenancy.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Platform.Tenancy.Infrastructure.Configurations;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages", "platform");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.EventType).HasMaxLength(256).IsRequired();
        builder.Property(x => x.Payload).HasColumnType("jsonb").IsRequired();
        builder.Property(x => x.Error).HasMaxLength(4000);
        builder.HasIndex(x => x.ProcessedAt);
        builder.HasIndex(x => x.TenantId);
    }
}
""")

platform_auditable = [
    ("Organization", "Subsidiary", "organization", "subsidiaries",
     '        builder.Property(x => x.Code).HasMaxLength(64);\n        builder.Property(x => x.Name).HasMaxLength(256);\n        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();'),
    ("Organization", "Branch", "organization", "branches",
     '        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.Code }).IsUnique();'),
    ("Organization", "Department", "organization", "departments",
     '        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.Code }).IsUnique();'),
    ("Organization", "CostCenter", "organization", "cost_centers",
     '        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.Code }).IsUnique();'),
    ("Organization", "ReportingDimension", "organization", "reporting_dimensions",
     '        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();'),
    ("Settings", "SystemSetting", "organization", "system_settings",
     '        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.Key }).IsUnique();'),
    ("Settings", "DocumentSequence", "organization", "document_sequences",
     '        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.EntityName }).IsUnique();\n        builder.Property(x => x.EntityName).HasMaxLength(128);'),
    ("Identity", "AppUser", "identity", "users",
     '        builder.Property(x => x.Email).HasMaxLength(256);\n        builder.HasIndex(x => new { x.TenantId, x.Email }).IsUnique();'),
    ("Identity", "AppRole", "identity", "roles",
     '        builder.HasIndex(x => new { x.TenantId, x.Name }).IsUnique();'),
    ("Identity", "Permission", "identity", "permissions",
     '        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();'),
    ("Identity", "UserRole", "identity", "user_roles",
     '        builder.HasIndex(x => new { x.TenantId, x.UserId, x.RoleId }).IsUnique();'),
    ("Identity", "RolePermission", "identity", "role_permissions",
     '        builder.HasIndex(x => new { x.TenantId, x.RoleId, x.PermissionId }).IsUnique();'),
    ("Identity", "UserSubsidiary", "identity", "user_subsidiaries",
     '        builder.HasIndex(x => new { x.TenantId, x.UserId, x.SubsidiaryId }).IsUnique();'),
    ("Identity", "RefreshToken", "identity", "refresh_tokens",
     '        builder.HasIndex(x => new { x.TenantId, x.TokenHash }).IsUnique();'),
    ("Identity", "FieldPermissionGrant", "identity", "field_permission_grants",
     '        builder.HasIndex(x => new { x.TenantId, x.RoleId, x.EntityName, x.FieldKey }).IsUnique();'),
    ("Audit", "ActivityLog", "audit", "activity_logs",
     '        builder.Property(x => x.OldValue).HasColumnType("jsonb");\n        builder.Property(x => x.NewValue).HasColumnType("jsonb");\n        builder.HasIndex(x => new { x.TenantId, x.EntityName, x.RecordId });'),
    ("Attachments", "Attachment", "attachments", "attachments",
     '        builder.HasIndex(x => new { x.TenantId, x.EntityName, x.RecordId });'),
    ("Notifications", "NotificationLog", "notifications", "notification_logs",
     '        builder.HasIndex(x => new { x.TenantId, x.UserId, x.CreatedAt });'),
    ("Metadata", "ModuleDefinition", "metadata", "module_definitions",
     '        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();'),
    ("Metadata", "ScreenDefinition", "metadata", "screen_definitions",
     '        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();\n        builder.HasOne(x => x.ModuleDefinition).WithMany(x => x.Screens).HasForeignKey(x => x.ModuleDefinitionId);'),
    ("Metadata", "SectionDefinition", "metadata", "section_definitions",
     '        builder.HasOne(x => x.ScreenDefinition).WithMany(x => x.Sections).HasForeignKey(x => x.ScreenDefinitionId);'),
    ("Metadata", "FieldDefinition", "metadata", "field_definitions",
     '        builder.HasOne(x => x.SectionDefinition).WithMany(x => x.Fields).HasForeignKey(x => x.SectionDefinitionId);'),
    ("Metadata", "CustomFieldDefinition", "metadata", "custom_field_definitions",
     '        builder.HasIndex(x => new { x.TenantId, x.EntityName, x.FieldKey }).IsUnique();'),
    ("Metadata", "CustomFieldValue", "metadata", "custom_field_values",
     '        builder.Property(x => x.ValueJson).HasColumnType("jsonb");\n        builder.HasIndex(x => new { x.TenantId, x.EntityName, x.RecordId, x.FieldKey }).IsUnique();'),
    ("DynamicModules", "DynamicModuleDefinition", "dynamic", "dynamic_module_definitions",
     '        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();'),
    ("DynamicModules", "DynamicEntityDefinition", "dynamic", "dynamic_entity_definitions",
     '        builder.HasIndex(x => new { x.TenantId, x.EntityName }).IsUnique();'),
    ("DynamicModules", "DynamicFieldDefinition", "dynamic", "dynamic_field_definitions",
     '        builder.HasIndex(x => new { x.TenantId, x.DynamicEntityDefinitionId, x.FieldKey }).IsUnique();'),
    ("DynamicModules", "DynamicRecord", "dynamic", "dynamic_records",
     '        builder.Property(x => x.PayloadJson).HasColumnType("jsonb");\n        builder.HasIndex(x => new { x.TenantId, x.EntityName });'),
]

for area, entity, schema, table, extras in platform_auditable:
    write(
        f"{P}/{area}/Infrastructure/Configurations/{entity}Configuration.cs",
        f"""
using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.{area}.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Platform.{area}.Infrastructure.Configurations;

public sealed class {entity}Configuration : AuditableEntityConfiguration<{entity}>
{{
    public override void Configure(EntityTypeBuilder<{entity}> builder)
    {{
        base.Configure(builder);
        builder.ToTable("{table}", "{schema}");
{extras}
    }}
}}
""")

# Platform DbContexts - group related
write(f"{P}/Tenancy/Infrastructure/PlatformDbContext.cs", """
using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.Attachments.Domain;
using iERP.Modules.Platform.Audit.Domain;
using iERP.Modules.Platform.DynamicModules.Domain;
using iERP.Modules.Platform.Notifications.Domain;
using iERP.Modules.Platform.Tenancy.Domain;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.Platform.Tenancy.Infrastructure;

public sealed class PlatformDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public PlatformDbContext(DbContextOptions<PlatformDbContext> options, ITenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<NotificationLog> NotificationLogs => Set<NotificationLog>();
    public DbSet<DynamicModuleDefinition> DynamicModuleDefinitions => Set<DynamicModuleDefinition>();
    public DbSet<DynamicEntityDefinition> DynamicEntityDefinitions => Set<DynamicEntityDefinition>();
    public DbSet<DynamicFieldDefinition> DynamicFieldDefinitions => Set<DynamicFieldDefinition>();
    public DbSet<DynamicRecord> DynamicRecords => Set<DynamicRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("platform");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PlatformDbContext).Assembly);
        modelBuilder.ApplySnakeCaseNamingConvention();
        modelBuilder.ConfigureMoneyPrecision();
        modelBuilder.ApplyTenantAndSoftDeleteFilters(_tenantContext);
        base.OnModelCreating(modelBuilder);
    }
}
""")

write(f"{P}/Identity/Infrastructure/IdentityDbContext.cs", """
using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.Identity.Domain;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.Platform.Identity.Infrastructure;

public sealed class IdentityDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public IdentityDbContext(DbContextOptions<IdentityDbContext> options, ITenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<AppRole> Roles => Set<AppRole>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserSubsidiary> UserSubsidiaries => Set<UserSubsidiary>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<FieldPermissionGrant> FieldPermissionGrants => Set<FieldPermissionGrant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("identity");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
        modelBuilder.ApplySnakeCaseNamingConvention();
        modelBuilder.ConfigureMoneyPrecision();
        modelBuilder.ApplyTenantAndSoftDeleteFilters(_tenantContext);
        base.OnModelCreating(modelBuilder);
    }
}
""")

write(f"{P}/Organization/Infrastructure/OrganizationDbContext.cs", """
using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.Organization.Domain;
using iERP.Modules.Platform.Settings.Domain;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.Platform.Organization.Infrastructure;

public sealed class OrganizationDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public OrganizationDbContext(DbContextOptions<OrganizationDbContext> options, ITenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<Subsidiary> Subsidiaries => Set<Subsidiary>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<CostCenter> CostCenters => Set<CostCenter>();
    public DbSet<ReportingDimension> ReportingDimensions => Set<ReportingDimension>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
    public DbSet<DocumentSequence> DocumentSequences => Set<DocumentSequence>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("organization");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrganizationDbContext).Assembly);
        modelBuilder.ApplySnakeCaseNamingConvention();
        modelBuilder.ConfigureMoneyPrecision();
        modelBuilder.ApplyTenantAndSoftDeleteFilters(_tenantContext);
        base.OnModelCreating(modelBuilder);
    }
}
""")

write(f"{P}/Metadata/Infrastructure/MetadataDbContext.cs", """
using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.Metadata.Domain;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.Platform.Metadata.Infrastructure;

public sealed class MetadataDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public MetadataDbContext(DbContextOptions<MetadataDbContext> options, ITenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<ModuleDefinition> ModuleDefinitions => Set<ModuleDefinition>();
    public DbSet<ScreenDefinition> ScreenDefinitions => Set<ScreenDefinition>();
    public DbSet<SectionDefinition> SectionDefinitions => Set<SectionDefinition>();
    public DbSet<FieldDefinition> FieldDefinitions => Set<FieldDefinition>();
    public DbSet<CustomFieldDefinition> CustomFieldDefinitions => Set<CustomFieldDefinition>();
    public DbSet<CustomFieldValue> CustomFieldValues => Set<CustomFieldValue>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("metadata");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MetadataDbContext).Assembly);
        modelBuilder.ApplySnakeCaseNamingConvention();
        modelBuilder.ConfigureMoneyPrecision();
        modelBuilder.ApplyTenantAndSoftDeleteFilters(_tenantContext);
        base.OnModelCreating(modelBuilder);
    }
}
""")

# Seeders
write(f"{P}/Identity/Application/Seeding/SystemPermissionSeeder.cs", """
using iERP.Application.Abstractions.Seeding;
using iERP.Modules.Platform.Identity.Domain;
using iERP.Modules.Platform.Identity.Infrastructure;
using iERP.SharedKernel.Security;
using iERP.SharedKernel.Time;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.Platform.Identity.Application.Seeding;

public sealed class SystemPermissionSeeder : IDataSeeder
{
    private readonly IdentityDbContext _db;
    private readonly IClock _clock;

    public SystemPermissionSeeder(IdentityDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        // System permission catalog is global-per-tenant at runtime; here we only ensure codes exist conceptually.
        // Full tenant onboarding seeding is deferred.
        await Task.CompletedTask;
        _ = (_db, _clock, Permissions.Crm.LeadRead);
    }
}
""")

write(f"{P}/Identity/Application/Seeding/SystemRoleSeeder.cs", """
using iERP.Application.Abstractions.Seeding;
using iERP.SharedKernel.Security;

namespace iERP.Modules.Platform.Identity.Application.Seeding;

public sealed class SystemRoleSeeder : IDataSeeder
{
    public Task SeedAsync(CancellationToken cancellationToken = default)
    {
        _ = SystemRoles.All;
        return Task.CompletedTask;
    }
}
""")

write(f"{P}/Metadata/Application/Seeding/DefaultMetadataSeeder.cs", """
using iERP.Application.Abstractions.Seeding;

namespace iERP.Modules.Platform.Metadata.Application.Seeding;

public sealed class DefaultMetadataSeeder : IDataSeeder
{
    public Task SeedAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
""")

# Auth endpoints placeholder
write(f"{P}/Identity/Api/AuthEndpoints.cs", """
using iERP.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace iERP.Modules.Platform.Identity.Api;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/auth").WithTags("Auth");

        group.MapPost("/login", () =>
            Results.Json(ApiErrorResponse.Create("NOT_IMPLEMENTED", "Login endpoint is not implemented yet."), statusCode: StatusCodes.Status501NotImplemented));

        group.MapPost("/refresh", () =>
            Results.Json(ApiErrorResponse.Create("NOT_IMPLEMENTED", "Refresh endpoint is not implemented yet."), statusCode: StatusCodes.Status501NotImplemented));

        group.MapPost("/logout", () =>
            Results.Json(ApiErrorResponse.Create("NOT_IMPLEMENTED", "Logout endpoint is not implemented yet."), statusCode: StatusCodes.Status501NotImplemented));

        return app;
    }
}
""")

write(f"{P}/Tenancy/Api/TenancyEndpoints.cs", endpoint("iERP.Modules.Platform.Tenancy.Api", "Tenants", "tenants"))
write(f"{P}/Organization/Api/OrganizationEndpoints.cs", endpoint("iERP.Modules.Platform.Organization.Api", "Organization", "organization"))
write(f"{P}/Metadata/Api/MetadataEndpoints.cs", endpoint("iERP.Modules.Platform.Metadata.Api", "Metadata", "metadata"))
write(f"{P}/Settings/Api/SettingsEndpoints.cs", endpoint("iERP.Modules.Platform.Settings.Api", "Settings", "settings"))
write(f"{P}/Audit/Api/AuditEndpoints.cs", endpoint("iERP.Modules.Platform.Audit.Api", "Audit", "audit"))
write(f"{P}/Attachments/Api/AttachmentsEndpoints.cs", endpoint("iERP.Modules.Platform.Attachments.Api", "Attachments", "attachments"))
write(f"{P}/Notifications/Api/NotificationsEndpoints.cs", endpoint("iERP.Modules.Platform.Notifications.Api", "Notifications", "notifications"))
write(f"{P}/DynamicModules/Api/DynamicModulesEndpoints.cs", endpoint("iERP.Modules.Platform.DynamicModules.Api", "DynamicModules", "dynamic_modules"))

write(f"{P}/DependencyInjection.cs", """
using iERP.Application.Abstractions.Seeding;
using iERP.Infrastructure.Persistence.Interceptors;
using iERP.Modules.Platform.Identity.Application.Seeding;
using iERP.Modules.Platform.Identity.Infrastructure;
using iERP.Modules.Platform.Metadata.Application.Seeding;
using iERP.Modules.Platform.Metadata.Infrastructure;
using iERP.Modules.Platform.Organization.Infrastructure;
using iERP.Modules.Platform.Tenancy.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace iERP.Modules.Platform;

public static class DependencyInjection
{
    public static IServiceCollection AddPlatformModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PrimaryDatabase")
            ?? "Host=localhost;Port=5432;Database=ierp;Username=ierp;Password=ierp";

        services.AddDbContext<PlatformDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, b => b.MigrationsAssembly("iERP.Migrations"));
            options.AddInterceptors(
                sp.GetRequiredService<TenantSaveChangesInterceptor>(),
                sp.GetRequiredService<AuditSaveChangesInterceptor>());
        });

        services.AddDbContext<IdentityDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, b => b.MigrationsAssembly("iERP.Migrations"));
            options.AddInterceptors(
                sp.GetRequiredService<TenantSaveChangesInterceptor>(),
                sp.GetRequiredService<AuditSaveChangesInterceptor>());
        });

        services.AddDbContext<OrganizationDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, b => b.MigrationsAssembly("iERP.Migrations"));
            options.AddInterceptors(
                sp.GetRequiredService<TenantSaveChangesInterceptor>(),
                sp.GetRequiredService<AuditSaveChangesInterceptor>());
        });

        services.AddDbContext<MetadataDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, b => b.MigrationsAssembly("iERP.Migrations"));
            options.AddInterceptors(
                sp.GetRequiredService<TenantSaveChangesInterceptor>(),
                sp.GetRequiredService<AuditSaveChangesInterceptor>());
        });

        services.AddScoped<IDataSeeder, SystemRoleSeeder>();
        services.AddScoped<IDataSeeder, SystemPermissionSeeder>();
        services.AddScoped<IDataSeeder, DefaultMetadataSeeder>();

        return services;
    }
}
""")

write(f"{P}/Api/PlatformModuleEndpoints.cs", """
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
""")

print("platform ef/di done")
