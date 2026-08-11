using System.Data.Common;
using iERP.Application.Abstractions.Options;
using iERP.Application.Abstractions.Reporting;
using Microsoft.Extensions.Options;
using Npgsql;

namespace iERP.Infrastructure.Reporting;

public sealed class ReportingDbConnectionFactory : IReportingDbConnectionFactory
{
    private readonly DatabaseOptions _options;

    public ReportingDbConnectionFactory(IOptions<DatabaseOptions> options)
    {
        _options = options.Value;
    }

    public async Task<DbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connectionString = string.IsNullOrWhiteSpace(_options.ReportingDatabase)
            ? _options.PrimaryDatabase
            : _options.ReportingDatabase;

        var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
