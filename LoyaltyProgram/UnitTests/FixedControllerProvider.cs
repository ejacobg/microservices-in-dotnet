using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace UnitTests
{
    public class FixedControllerProvider : ControllerFeatureProvider
    {
        private readonly Type[] _controllerTypes;

        public FixedControllerProvider(params Type[] controllerTypes) => _controllerTypes = controllerTypes;

        // Override the method that is used to detect controller in your application.
        // We will only return true for controllers that we specifically want to be recognized.
        protected override bool IsController(TypeInfo typeInfo) => _controllerTypes.Contains(typeInfo);
    }

    public static class MvcBuilderExtensions
    {
        // Add a new extension method to the service builder.
        public static IMvcBuilder
            AddControllersByType(this IServiceCollection services, params Type[] controllerTypes) =>
            services
                .AddControllers() // Make the service collection aware of controllers.
                .ConfigureApplicationPartManager(mgr => // Get access to feature providers.
                {
                    // Remove the default controller provider.
                    mgr.FeatureProviders.Remove(mgr.FeatureProviders.First(f => f is ControllerFeatureProvider));

                    // Add in our custom controller provider.
                    mgr.FeatureProviders.Add(new FixedControllerProvider(controllerTypes));
                });
    }
}