namespace iERP.Application.Abstractions.Options;

public sealed class HangfireOptions
{
    public const string SectionName = "Hangfire";

    public bool Enabled { get; set; } = true;
    public string SchemaName { get; set; } = "hangfire";
    public int WorkerCount { get; set; } = 2;
}
