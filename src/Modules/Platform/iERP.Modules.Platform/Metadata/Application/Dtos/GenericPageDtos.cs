namespace iERP.Modules.Platform.Metadata.Application.Dtos;

/// <summary>
/// ProcessFlow v4 GenericPage contract returned by GET /api/v1/metadata/screens/{screenCode}.
/// </summary>
public sealed class GenericPageDto
{
    public GenericPageScreenDto Screen { get; init; } = new();
    public GenericPageLayoutDto Layout { get; init; } = new();
    public IReadOnlyList<GenericPageSectionDto> Sections { get; init; } = [];
    public IReadOnlyList<GenericPageActionDto> Actions { get; init; } = [];
}

public sealed class GenericPageScreenDto
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Route { get; init; } = string.Empty;
    public string RenderMode { get; init; } = "generic";
    public string EntityName { get; init; } = string.Empty;
    public string ApiBasePath { get; init; } = string.Empty;
    public bool WorkflowEnabled { get; init; }
    public bool PrintEnabled { get; init; }
    public bool AiEnabled { get; init; }
}

public sealed class GenericPageLayoutDto
{
    public string Mode { get; init; } = "form-with-grid";
    public int Columns { get; init; } = 12;
}

public sealed class GenericPageSectionDto
{
    public string Code { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Type { get; init; } = "header";
    public IReadOnlyList<GenericPageFieldDto> Fields { get; init; } = [];
}

public sealed class GenericPageFieldDto
{
    public string FieldKey { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public string DataType { get; init; } = "text";
    public string ControlType { get; init; } = "input";
    public bool Required { get; init; }
    public bool ReadOnly { get; init; }
    public bool Visible { get; init; } = true;
    public int Width { get; init; } = 3;
    public int DisplayOrder { get; init; }
    public bool IsCustom { get; init; }
}

public sealed class GenericPageActionDto
{
    public string ActionKey { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public string ActionType { get; init; } = "api";
    public string Endpoint { get; init; } = string.Empty;
}
