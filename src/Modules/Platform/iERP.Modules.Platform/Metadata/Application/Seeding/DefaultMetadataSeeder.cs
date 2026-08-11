using iERP.Application.Abstractions.Seeding;

namespace iERP.Modules.Platform.Metadata.Application.Seeding;

public sealed class DefaultMetadataSeeder : IDataSeeder
{
    public Task SeedAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
