using iERP.Application.Abstractions.Seeding;
using iERP.Modules.Platform.Identity.Domain;
using iERP.Modules.Platform.Identity.Infrastructure;
using iERP.SharedKernel.Security;
using iERP.SharedKernel.Time;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.Platform.Identity.Application.Seeding;

public sealed class SystemPermissionSeeder : IDataSeeder
{
    private readonly IdentityDbContext _db;
    private readonly IClock _clock;

    public SystemPermissionSeeder(IdentityDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        // System permission catalog is global-per-tenant at runtime; here we only ensure codes exist conceptually.
        // Full tenant onboarding seeding is deferred.
        await Task.CompletedTask;
        _ = (_db, _clock, Permissions.Crm.LeadRead);
    }
}
