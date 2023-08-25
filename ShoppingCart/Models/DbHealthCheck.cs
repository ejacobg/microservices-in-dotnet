using System.Data.SqlClient;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace ShoppingCart.Models
{
    public class DbHealthCheck : IHealthCheck
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public DbHealthCheck(ILoggerFactory logger)
        {
            _httpClient = new HttpClient();
            _logger = logger.CreateLogger<DbHealthCheck>();
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken cancellationToken)
        {
            await using var conn = // Open a new database connection.
                new SqlConnection(
                    "Data Source=localhost;Initial Catalog=master;User Id=SA; Password=yourStrong(!)Password");
            var result = await conn.QuerySingleAsync<int>("SELECT 1");

            // If the query was successful, then we are able to connect to the database.
            return result == 1
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Degraded();
        }
    }
}