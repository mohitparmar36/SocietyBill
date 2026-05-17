using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace SocietyBill.API.Middlewares
{
    public class TenantIdentifierMiddleware
    {
        private readonly RequestDelegate _next;
        public TenantIdentifierMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Ensure society_id claim is present for multi-tenancy
            var societyClaim = context.User.Claims.FirstOrDefault(c => c.Type == "society_id");
            if (societyClaim == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("SocietyId claim missing in token.");
                return;
            }
            await _next(context);
        }
    }
}