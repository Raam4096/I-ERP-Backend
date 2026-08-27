namespace iERP.Modules.Platform.Metadata.Application.Dtos;

public sealed class MetadataModuleDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsActive { get; init; }
    public string Source { get; init; } = "metadata";
    public IReadOnlyList<MetadataScreenSummaryDto> Screens { get; init; } = [];
}

public sealed class MetadataScreenSummaryDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Route { get; init; } = string.Empty;
    public string EntityName { get; init; } = string.Empty;
    public string ApiBasePath { get; init; } = string.Empty;
}

public sealed class ScreenFieldPreferenceItemDto
{
    public string FieldKey { get; set; } = string.Empty;
    public bool IsVisible { get; set; } = true;
    public int DisplayOrder { get; set; }
}

public sealed class SaveScreenFieldPreferencesRequest
{
    public List<ScreenFieldPreferenceItemDto> Fields { get; set; } = [];
}

public sealed class CustomFieldDefinitionDto
{
    public Guid Id { get; init; }
    public string EntityName { get; init; } = string.Empty;
    public string FieldKey { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public string DataType { get; init; } = "string";
    public int DisplayOrder { get; init; }
    public bool IsRequired { get; init; }
    public bool IsActive { get; init; }
}

public sealed class CreateCustomFieldDefinitionRequest
{
    public string FieldKey { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string DataType { get; set; } = "string";
    public int DisplayOrder { get; set; }
    public bool IsRequired { get; set; }
    public bool IsActive { get; set; } = true;
}

public sealed class UpdateCustomFieldDefinitionRequest
{
    public string Label { get; set; } = string.Empty;
    public string DataType { get; set; } = "string";
    public int DisplayOrder { get; set; }
    public bool IsRequired { get; set; }
    public bool IsActive { get; set; } = true;
}
