using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace LoyaltyProgram
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // This service will enable reading and validating of identity tokens (as JWTs) from incoming request headers.
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o => o.TokenValidationParameters = new TokenValidationParameters
                {
                    // DO NOT USE THIS CONFIGURATION IN PRODUCTION
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = false,
                    SignatureValidator = (token, parameters) => new JwtSecurityToken(token),
                });
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();
            app.UseRouting();
            // Enable usage of the [Authorize] attribute on your action methods,
            // which will require an identity token to be present in the request.
            // Controllers can access the user data from the ControllerBase.User property.
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}