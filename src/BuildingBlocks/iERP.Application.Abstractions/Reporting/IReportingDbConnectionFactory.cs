using System.Data.Common;

namespace iERP.Application.Abstractions.Reporting;

public interface IReportingDbConnectionFactory
{
    Task<DbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default);
}
