using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using ShoppingCart.Models;

namespace ShoppingCart
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Use the Scrutor package to automatically bind implementations to their corresponding interfaces.
            services.Scan(selector =>
                selector
                    .FromAssemblyOf<Startup>()
                    // Copied from original repo. Not exactly sure what the implementation type filter does, but it is needed to run the project.
                    .AddClasses(c => c.Where(t => t != typeof(ProductCatalogClient) && t.GetMethods().All(m => m.Name != "<Clone>$")))
                    .AsImplementedInterfaces());

            // Inject a typed HTTP client into our ProductCatalogClient.
            services.AddHttpClient<IProductCatalogClient, ProductCatalogClient>()
                // Use the Polly package to automatically retry failed requests.
                .AddTransientHttpErrorPolicy(policy =>
                    // Set up a retry policy with exponential backoff (each waiting period is twice as long as the last).
                    policy.WaitAndRetryAsync(
                        3,
                        attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt))));
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}