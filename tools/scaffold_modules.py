#!/usr/bin/env python3
"""Generate module entities, EF configs, DbContexts, DI, and endpoint maps."""
from __future__ import annotations

from pathlib import Path
from textwrap import dedent

ROOT = Path(__file__).resolve().parents[1]


def write(rel: str, content: str) -> None:
    path = ROOT / rel
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(dedent(content).lstrip("\n").replace("\r\n", "\n"), encoding="utf-8")
    print(rel)


def entity_file(ns: str, class_name: str, body: str, base: str = "AuditableEntity") -> str:
    return f"""
using iERP.SharedKernel.Primitives;

namespace {ns};

public sealed class {class_name} : {base}
{{
{body}
}}
"""


def config_file(ns: str, entity: str, schema: str, table: str, extra: str = "") -> str:
    return f"""
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using iERP.Infrastructure.Persistence;

namespace {ns};

public sealed class {entity}Configuration : AuditableEntityConfiguration<{entity}>
{{
    public override void Configure(EntityTypeBuilder<{entity}> builder)
    {{
        base.Configure(builder);
        builder.ToTable("{table}", "{schema}");
{extra}
    }}
}}
"""


def module_health_endpoints(ns: str, route: str, module_name: str) -> str:
    return f"""
using iERP.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace {ns};

public static class {module_name}Endpoints
{{
    public static IEndpointRouteBuilder Map{module_name}Endpoints(this IEndpointRouteBuilder app)
    {{
        var group = app.MapGroup("/api/v1/{route}").WithTags("{module_name}");
        group.MapGet("/health", () => Results.Ok(ApiResponse<string>.Ok("{module_name} module ready")))
            .WithName("{module_name}Health");
        return app;
    }}
}}
"""


# ===================== PLATFORM =====================
P = "src/Modules/Platform/iERP.Modules.Platform"

write(f"{P}/Tenancy/Domain/Tenant.cs", """
using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Platform.Tenancy.Domain;

/// <summary>
/// SaaS customer root. Not tenant-scoped (no tenant_id on itself).
/// </summary>
public sealed class Tenant : Entity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = "active";
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
""")

write(f"{P}/Tenancy/Domain/OutboxMessage.cs", """
using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Platform.Tenancy.Domain;

public sealed class OutboxMessage : Entity
{
    public Guid? TenantId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Payload { get; set; } = "{}";
    public DateTimeOffset OccurredAt { get; set; }
    public DateTimeOffset? ProcessedAt { get; set; }
    public int RetryCount { get; set; }
    public string? Error { get; set; }
}
""")

write(f"{P}/Organization/Domain/Subsidiary.cs", entity_file(
    "iERP.Modules.Platform.Organization.Domain",
    "Subsidiary",
    """
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? LegalName { get; set; }
    public string? TaxRegistrationNumber { get; set; }
    public string? Country { get; set; }
    public string? CurrencyCode { get; set; }
    public bool IsActive { get; set; } = true;
"""))

write(f"{P}/Organization/Domain/Branch.cs", entity_file(
    "iERP.Modules.Platform.Organization.Domain",
    "Branch",
    """
    public Guid SubsidiaryId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? AddressLine { get; set; }
    public bool IsActive { get; set; } = true;
"""))

write(f"{P}/Organization/Domain/Department.cs", entity_file(
    "iERP.Modules.Platform.Organization.Domain",
    "Department",
    """
    public Guid SubsidiaryId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
"""))

write(f"{P}/Organization/Domain/CostCenter.cs", entity_file(
    "iERP.Modules.Platform.Organization.Domain",
    "CostCenter",
    """
    public Guid SubsidiaryId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
"""))

write(f"{P}/Organization/Domain/ReportingDimension.cs", entity_file(
    "iERP.Modules.Platform.Organization.Domain",
    "ReportingDimension",
    """
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DimensionType { get; set; } = "class";
    public bool IsActive { get; set; } = true;
"""))

write(f"{P}/Settings/Domain/SystemSetting.cs", entity_file(
    "iERP.Modules.Platform.Settings.Domain",
    "SystemSetting",
    """
    public Guid? SubsidiaryId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
"""))

write(f"{P}/Settings/Domain/DocumentSequence.cs", entity_file(
    "iERP.Modules.Platform.Settings.Domain",
    "DocumentSequence",
    """
    public Guid SubsidiaryId { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string Prefix { get; set; } = string.Empty;
    public long NextNumber { get; set; } = 1;
    public int Padding { get; set; } = 6;
"""))

# Identity entities
write(f"{P}/Identity/Domain/AppUser.cs", entity_file(
    "iERP.Modules.Platform.Identity.Domain",
    "AppUser",
    """
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset? LastLoginAt { get; set; }
"""))

write(f"{P}/Identity/Domain/AppRole.cs", entity_file(
    "iERP.Modules.Platform.Identity.Domain",
    "AppRole",
    """
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }
"""))

write(f"{P}/Identity/Domain/Permission.cs", entity_file(
    "iERP.Modules.Platform.Identity.Domain",
    "Permission",
    """
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Module { get; set; }
    public string? Description { get; set; }
"""))

write(f"{P}/Identity/Domain/UserRole.cs", entity_file(
    "iERP.Modules.Platform.Identity.Domain",
    "UserRole",
    """
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
"""))

write(f"{P}/Identity/Domain/RolePermission.cs", entity_file(
    "iERP.Modules.Platform.Identity.Domain",
    "RolePermission",
    """
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
"""))

write(f"{P}/Identity/Domain/UserSubsidiary.cs", entity_file(
    "iERP.Modules.Platform.Identity.Domain",
    "UserSubsidiary",
    """
    public Guid UserId { get; set; }
    public Guid SubsidiaryId { get; set; }
    public bool IsDefault { get; set; }
"""))

write(f"{P}/Identity/Domain/RefreshToken.cs", entity_file(
    "iERP.Modules.Platform.Identity.Domain",
    "RefreshToken",
    """
    public Guid UserId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public string? ReplacedByTokenHash { get; set; }
"""))

write(f"{P}/Identity/Domain/FieldPermissionGrant.cs", entity_file(
    "iERP.Modules.Platform.Identity.Domain",
    "FieldPermissionGrant",
    """
    public Guid RoleId { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string FieldKey { get; set; } = string.Empty;
    public bool CanView { get; set; } = true;
    public bool CanEdit { get; set; }
"""))

# Audit, Attachments, Notifications, Metadata, Dynamic
write(f"{P}/Audit/Domain/ActivityLog.cs", entity_file(
    "iERP.Modules.Platform.Audit.Domain",
    "ActivityLog",
    """
    public Guid? UserId { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public Guid RecordId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? IpAddress { get; set; }
"""))

write(f"{P}/Attachments/Domain/Attachment.cs", entity_file(
    "iERP.Modules.Platform.Attachments.Domain",
    "Attachment",
    """
    public string EntityName { get; set; } = string.Empty;
    public Guid RecordId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string BlobPath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public Guid UploadedBy { get; set; }
"""))

write(f"{P}/Notifications/Domain/NotificationLog.cs", entity_file(
    "iERP.Modules.Platform.Notifications.Domain",
    "NotificationLog",
    """
    public Guid? UserId { get; set; }
    public string Channel { get; set; } = "email";
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Status { get; set; } = "pending";
    public string? Error { get; set; }
    public DateTimeOffset? SentAt { get; set; }
"""))

write(f"{P}/Metadata/Domain/ModuleDefinition.cs", entity_file(
    "iERP.Modules.Platform.Metadata.Domain",
    "ModuleDefinition",
    """
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<ScreenDefinition> Screens { get; set; } = new List<ScreenDefinition>();
"""))

write(f"{P}/Metadata/Domain/ScreenDefinition.cs", entity_file(
    "iERP.Modules.Platform.Metadata.Domain",
    "ScreenDefinition",
    """
    public Guid ModuleDefinitionId { get; set; }
    public ModuleDefinition? ModuleDefinition { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public string RenderMode { get; set; } = "standard";
    public string EntityName { get; set; } = string.Empty;
    public string ApiBasePath { get; set; } = string.Empty;
    public bool WorkflowEnabled { get; set; }
    public bool PrintEnabled { get; set; }
    public bool AiEnabled { get; set; }
    public ICollection<SectionDefinition> Sections { get; set; } = new List<SectionDefinition>();
"""))

write(f"{P}/Metadata/Domain/SectionDefinition.cs", entity_file(
    "iERP.Modules.Platform.Metadata.Domain",
    "SectionDefinition",
    """
    public Guid ScreenDefinitionId { get; set; }
    public ScreenDefinition? ScreenDefinition { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public ICollection<FieldDefinition> Fields { get; set; } = new List<FieldDefinition>();
"""))

write(f"{P}/Metadata/Domain/FieldDefinition.cs", entity_file(
    "iERP.Modules.Platform.Metadata.Domain",
    "FieldDefinition",
    """
    public Guid SectionDefinitionId { get; set; }
    public SectionDefinition? SectionDefinition { get; set; }
    public string FieldKey { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string DataType { get; set; } = "string";
    public string ControlType { get; set; } = "text";
    public int DisplayOrder { get; set; }
    public bool IsRequired { get; set; }
    public bool IsReadOnly { get; set; }
    public bool IsVisible { get; set; } = true;
    public int? Width { get; set; }
"""))

write(f"{P}/Metadata/Domain/CustomFieldDefinition.cs", entity_file(
    "iERP.Modules.Platform.Metadata.Domain",
    "CustomFieldDefinition",
    """
    public string EntityName { get; set; } = string.Empty;
    public string FieldKey { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string DataType { get; set; } = "string";
    public int DisplayOrder { get; set; }
    public bool IsRequired { get; set; }
    public bool IsActive { get; set; } = true;
"""))

write(f"{P}/Metadata/Domain/CustomFieldValue.cs", entity_file(
    "iERP.Modules.Platform.Metadata.Domain",
    "CustomFieldValue",
    """
    public string EntityName { get; set; } = string.Empty;
    public Guid RecordId { get; set; }
    public string FieldKey { get; set; } = string.Empty;
    public string? ValueText { get; set; }
    public decimal? ValueNumber { get; set; }
    public DateTimeOffset? ValueDate { get; set; }
    public bool? ValueBoolean { get; set; }
    public string? ValueJson { get; set; }
"""))

write(f"{P}/DynamicModules/Domain/DynamicModuleDefinition.cs", entity_file(
    "iERP.Modules.Platform.DynamicModules.Domain",
    "DynamicModuleDefinition",
    """
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
"""))

write(f"{P}/DynamicModules/Domain/DynamicEntityDefinition.cs", entity_file(
    "iERP.Modules.Platform.DynamicModules.Domain",
    "DynamicEntityDefinition",
    """
    public Guid DynamicModuleDefinitionId { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
"""))

write(f"{P}/DynamicModules/Domain/DynamicFieldDefinition.cs", entity_file(
    "iERP.Modules.Platform.DynamicModules.Domain",
    "DynamicFieldDefinition",
    """
    public Guid DynamicEntityDefinitionId { get; set; }
    public string FieldKey { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string DataType { get; set; } = "string";
    public int DisplayOrder { get; set; }
    public bool IsRequired { get; set; }
"""))

write(f"{P}/DynamicModules/Domain/DynamicRecord.cs", entity_file(
    "iERP.Modules.Platform.DynamicModules.Domain",
    "DynamicRecord",
    """
    public Guid DynamicEntityDefinitionId { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = "{}";
"""))

print("platform entities done")
