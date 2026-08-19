namespace iERP.Modules.CRM.Application.Opportunities.Services;

public interface IOpportunityNumberGenerator
{
    Task<string> GenerateAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
