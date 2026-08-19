using iERP.Modules.CRM.Infrastructure;
using iERP.SharedKernel.Time;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.CRM.Application.Opportunities.Services;

public sealed class OpportunityNumberGenerator : IOpportunityNumberGenerator
{
    private readonly CrmDbContext _db;
    private readonly IClock _clock;

    public OpportunityNumberGenerator(CrmDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<string> GenerateAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var year = _clock.UtcNow.Year;
        var prefix = $"OPP-{year}-";

        // Include soft-deleted rows: unique index still reserves opportunity_number.
        var last = await _db.Opportunities
            .IgnoreQueryFilters()
            .Where(x => x.TenantId == tenantId && x.OpportunityNumber.StartsWith(prefix))
            .OrderByDescending(x => x.OpportunityNumber)
            .Select(x => x.OpportunityNumber)
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
