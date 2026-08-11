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
