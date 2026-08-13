namespace iERP.Modules.CRM.Application.Leads.Services;

public interface ILeadNumberGenerator
{
    Task<string> GenerateAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
