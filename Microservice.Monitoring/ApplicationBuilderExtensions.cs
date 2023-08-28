using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Microservice.Monitoring
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseKubernetesHealthChecks(this IApplicationBuilder app) =>
            // Register the two Kubernetes health check endpoints.
            app
                .UseHealthChecks("/health/startup",
                    new HealthCheckOptions { Predicate = x => x.Tags.Contains("startup") })
                .UseHealthChecks("/health/live",
                    new HealthCheckOptions { Predicate = x => x.Tags.Contains("liveness") });
    }
}