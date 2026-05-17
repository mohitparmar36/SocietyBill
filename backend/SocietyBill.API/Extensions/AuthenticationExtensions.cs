using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace SocietyBill.API.Extensions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddAuth0Authentication(this IServiceCollection services, IConfiguration configuration)
        {
            var domain = configuration["Auth0:Domain"];
            var audience = configuration["Auth0:Audience"];
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = $"https://{domain}/";
                options.Audience = audience;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "sub",
                    RoleClaimType = "https://societybill.com/roles"
                };
            });
            services.AddAuthorization();
            return services;
        }
    }
}