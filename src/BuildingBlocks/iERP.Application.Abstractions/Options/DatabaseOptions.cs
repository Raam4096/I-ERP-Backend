namespace iERP.Application.Abstractions.Options;

public sealed class DatabaseOptions
{
    public const string SectionName = "ConnectionStrings";

    public string PrimaryDatabase { get; set; } = string.Empty;
    public string ReportingDatabase { get; set; } = string.Empty;
}
