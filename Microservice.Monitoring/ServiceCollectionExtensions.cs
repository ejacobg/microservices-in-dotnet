using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Microservice.Monitoring
{
    public static class ServiceCollectionExtensions
    {
        private const string Liveness = "liveness";
        private const string Startup = "startup";

        public static IServiceCollection AddBasicHealthChecks(this IServiceCollection services)
        {
            // Add services for responding to health check endpoints.
            services.AddHealthChecks()
                // Register default checks.
                .AddCheck("BasicStasrtupHealthCheck", () => HealthCheckResult.Healthy(), tags: new[] { Startup })
                .AddCheck("BasicLivenessHealthCheck", () => HealthCheckResult.Healthy(), tags: new[] { Liveness });

            return services;
        }

        public static IServiceCollection AddAdditionStartupHealthChecks<T>(this IServiceCollection services)
            where T : class, IHealthCheck
        {
            services.AddHealthChecks().AddCheck<T>(nameof(T), tags: new[] { Startup });
            return services;
        }

        public static IServiceCollection AddAdditionLivenessHealthChecks<T>(this IServiceCollection services)
            where T : class, IHealthCheck
        {
            services.AddHealthChecks().AddCheck<T>(nameof(T), tags: new[] { Liveness });
            return services;
        }
    }
}