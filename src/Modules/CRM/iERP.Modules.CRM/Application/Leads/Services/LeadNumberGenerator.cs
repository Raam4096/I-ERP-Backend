using iERP.Modules.CRM.Infrastructure;
using iERP.SharedKernel.Time;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.CRM.Application.Leads.Services;

public sealed class LeadNumberGenerator : ILeadNumberGenerator
{
    private readonly CrmDbContext _db;
    private readonly IClock _clock;

    public LeadNumberGenerator(CrmDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<string> GenerateAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var year = _clock.UtcNow.Year;
        var prefix = $"LEAD-{year}-";

        var last = await _db.Leads
            .IgnoreQueryFilters()
            .Where(x => x.TenantId == tenantId && !x.IsDeleted && x.LeadNumber.StartsWith(prefix))
            .OrderByDescending(x => x.LeadNumber)
            .Select(x => x.LeadNumber)
            .FirstOrDefaultAsync(cancellationToken);

        var next = 1;
        if (!string.IsNullOrWhiteSpace(last) && last.Length >= prefix.Length + 6)
        {
            var suffix = last[prefix.Length..];
            if (int.TryParse(suffix, out var parsed))
            {
                next = parsed + 1;
            }
        }

        return $"{prefix}{next:D6}";
    }
}
