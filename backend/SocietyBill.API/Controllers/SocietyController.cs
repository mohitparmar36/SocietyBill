using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocietyBill.Application.DTOs.Request;
using SocietyBill.Application.Interfaces.Identity;
using SocietyBill.Application.Interfaces.Repositories;
using SocietyBill.Application.Interfaces.Services;
using SocietyBill.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SocietyBill.API.Controllers
{
    [ApiController]
    [Route("api/societies")]
    [Authorize] // Any authenticated user can register a society and become an admin
    public class SocietyController : ControllerBase
    {
        private readonly ISocietyRepository _societyRepository;
        private readonly IAuth0ManagementService _auth0ManagementService;
        private readonly ITenantProvider _tenantProvider;

        public SocietyController(
            ISocietyRepository societyRepository, 
            IAuth0ManagementService auth0ManagementService,
            ITenantProvider tenantProvider)
        {
            _societyRepository = societyRepository;
            _auth0ManagementService = auth0ManagementService;
            _tenantProvider = tenantProvider;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterSociety([FromBody] SocietyRegisterRequestDto request, CancellationToken cancellationToken)
        {
            var auth0UserId = _tenantProvider.UserId;
            if (string.IsNullOrEmpty(auth0UserId)) return Unauthorized();

            // If user already has a society, maybe block? But we can skip validation for simplicity.
            if (_tenantProvider.SocietyId != Guid.Empty)
            {
                return BadRequest("User already belongs to a society.");
            }

            var newSocietyId = Guid.NewGuid();

            var society = new Society
            {
                Id = newSocietyId,
                Name = request.Name,
                Address = request.Address,
                CreatedAt = DateTime.UtcNow
            };

            await _societyRepository.AddAsync(society, cancellationToken);
            
            // Upgrade user to Admin
            await _auth0ManagementService.UpdateUserAppMetadataAsync(auth0UserId, newSocietyId, cancellationToken);

            return Ok(new { Message = "Society registered successfully. Please refresh your token.", SocietyId = newSocietyId });
        }
    }
}
