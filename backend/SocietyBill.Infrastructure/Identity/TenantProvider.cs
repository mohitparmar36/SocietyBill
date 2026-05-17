using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SocietyBill.Application.Interfaces.Identity;

namespace SocietyBill.Infrastructure.Identity
{
    public class TenantProvider : ITenantProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TenantProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid SocietyId
        {
            get
            {
                var claim = _httpContextAccessor.HttpContext?.User.FindFirst("https://societybill.com/societyId");
                return claim != null && Guid.TryParse(claim.Value, out var id) ? id : Guid.Empty;
            }
        }

        public string UserId
        {
            get
            {
                var claim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
                return claim?.Value ?? string.Empty;
            }
        }

        public string Role
        {
            get
            {
                var claim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);
                return claim?.Value ?? string.Empty;
            }
        }
    }
}