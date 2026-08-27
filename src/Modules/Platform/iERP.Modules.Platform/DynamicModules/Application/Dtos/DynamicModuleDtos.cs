using System.Text.Json;

namespace iERP.Modules.Platform.DynamicModules.Application.Dtos;

public sealed class DynamicModuleDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsActive { get; init; }
    public IReadOnlyList<DynamicEntitySummaryDto> Entities { get; init; } = [];
}

public sealed class DynamicEntitySummaryDto
{
    public Guid Id { get; init; }
    public string EntityName { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}

public sealed class DynamicEntityDto
{
    public Guid Id { get; init; }
    public Guid ModuleId { get; init; }
    public string ModuleCode { get; init; } = string.Empty;
    public string EntityName { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public string ApiBasePath { get; init; } = string.Empty;
    public IReadOnlyList<DynamicFieldDto> Fields { get; init; } = [];
}

public sealed class DynamicFieldDto
{
    public Guid Id { get; init; }
    public Guid EntityId { get; init; }
    public string FieldKey { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public string DataType { get; init; } = "string";
    public string ControlType { get; init; } = "input";
    public int DisplayOrder { get; init; }
    public bool IsRequired { get; init; }
}

public sealed class DynamicRecordDto
{
    public Guid Id { get; init; }
    public Guid EntityId { get; init; }
    public string EntityName { get; init; } = string.Empty;
    public JsonElement Payload { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
}

public sealed class CreateDynamicModuleRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}

public sealed class UpdateDynamicModuleRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}

public sealed class CreateDynamicEntityRequest
{
    public string EntityName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public sealed class UpdateDynamicEntityRequest
{
    public string DisplayName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public sealed class CreateDynamicFieldRequest
{
    public string FieldKey { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string DataType { get; set; } = "string";
    public int DisplayOrder { get; set; }
    public bool IsRequired { get; set; }
}

public sealed class UpdateDynamicFieldRequest
{
    public string Label { get; set; } = string.Empty;
    public string DataType { get; set; } = "string";
    public int DisplayOrder { get; set; }
    public bool IsRequired { get; set; }
}

public sealed class UpsertDynamicRecordRequest
{
    public Dictionary<string, JsonElement>? Values { get; set; }
}
