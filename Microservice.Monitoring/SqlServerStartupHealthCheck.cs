using System.Data.SqlClient;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace Microservice.Monitoring
{
    public class SqlServerStartupHealthCheck : IHealthCheck
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public SqlServerStartupHealthCheck(ILoggerFactory logger)
        {
            _httpClient = new HttpClient();
            _logger = logger.CreateLogger<SqlServerStartupHealthCheck>();
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken cancellationToken)
        {
            // Uses a hardcoded connection string. This could be refactored to be injected into the health check.
            await using var conn =
                new SqlConnection(
                    "Data Source=localhost;Initial Catalog=master;User Id=SA; Password=yourStrong(!)Password");
            var result = await conn.QuerySingleAsync<int>("SELECT 1");
            return result == 1
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Degraded();
        }
    }
}