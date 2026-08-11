namespace iERP.Application.Abstractions.Options;

public sealed class AzureServiceBusOptions
{
    public const string SectionName = "AzureServiceBus";

    public bool Enabled { get; set; }
    public string ConnectionString { get; set; } = string.Empty;
    public string TopicName { get; set; } = "ierp-events";
}
