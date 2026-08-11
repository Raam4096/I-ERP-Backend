namespace iERP.Application.Abstractions.Options;

public sealed class AzureBlobStorageOptions
{
    public const string SectionName = "AzureBlobStorage";

    public bool Enabled { get; set; }
    public string ConnectionString { get; set; } = string.Empty;
    public string ContainerName { get; set; } = "ierp-attachments";
}
