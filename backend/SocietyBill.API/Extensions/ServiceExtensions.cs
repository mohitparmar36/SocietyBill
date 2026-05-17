using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SocietyBill.Infrastructure.Data;
using SocietyBill.Infrastructure.Identity;
using SocietyBill.Application.Interfaces.Identity;

namespace SocietyBill.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddSocietyBillServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ITenantProvider, TenantProvider>();
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
            // Register other services, repositories, etc. here
            return services;
        }
    }
}