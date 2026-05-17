using SocietyBill.Application.DTOs.Request;
using SocietyBill.Application.DTOs.Response;
using SocietyBill.Application.Interfaces.Repositories;
using SocietyBill.Application.Interfaces.Services;
using SocietyBill.Domain.Entities;
using SocietyBill.Application.Interfaces.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SocietyBill.Application.Services
{
    public class FlatService : IFlatService
    {
        private readonly IFlatRepository _flatRepository;
        private readonly ITenantProvider _tenantProvider;
        private readonly ISocietyRepository _societyRepository;
        private readonly IAuth0ManagementService _auth0ManagementService;

        public FlatService(
            IFlatRepository flatRepository, 
            ITenantProvider tenantProvider, 
            ISocietyRepository societyRepository,
            IAuth0ManagementService auth0ManagementService)
        {
            _flatRepository = flatRepository;
            _tenantProvider = tenantProvider;
            _societyRepository = societyRepository;
            _auth0ManagementService = auth0ManagementService;
        }

        public async Task<FlatResponseDto> CreateFlatAsync(FlatCreateRequestDto request, CancellationToken cancellationToken = default)
        {
            var societyId = _tenantProvider.SocietyId;
            
            if (societyId == Guid.Empty)
            {
                throw new UnauthorizedAccessException("Your token is missing the SocietyId claim. Please log out completely and log back in, or check your Auth0 Action script.");
            }
            
            // Ensure the Society exists to satisfy the foreign key constraint
            var existingSociety = await _societyRepository.GetByIdAsync(societyId, cancellationToken);
            if (existingSociety == null)
            {
                await _societyRepository.AddAsync(new Society
                {
                    Id = societyId,
                    Name = "My Society",
                    Address = "Default Address",
                    CreatedAt = DateTime.UtcNow
                }, cancellationToken);
            }

            var flatId = Guid.NewGuid();
            
            // Generate a random temporary password for the user
            var tempPassword = Guid.NewGuid().ToString("N") + "A1!";

            // Provision Auth0 User
            var auth0UserId = await _auth0ManagementService.CreateUserAsync(
                request.Email, 
                tempPassword, 
                societyId, 
                flatId, 
                cancellationToken);

            // Generate password reset / invite link
            var inviteLink = await _auth0ManagementService.CreatePasswordResetTicketAsync(auth0UserId, cancellationToken);

            var flat = new Flat
            {
                Id = flatId,
                FlatNumber = request.FlatNumber,
                OwnerName = request.OwnerName,
                Email = request.Email,
                Auth0UserId = auth0UserId,
                SocietyId = societyId,
                CreatedAt = DateTime.UtcNow
            };
            
            try
            {
                await _flatRepository.AddAsync(flat, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save flat with SocietyId={societyId}. ExistingSocietyFound={existingSociety != null}. TokenClaim={_tenantProvider.SocietyId}", ex);
            }
            
            return new FlatResponseDto(flat, inviteLink);
        }

        public async Task<List<FlatResponseDto>> GetFlatsForSocietyAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var flats = await _flatRepository.GetBySocietyAsync(_tenantProvider.SocietyId, page, pageSize, cancellationToken);
            return flats.Select(f => new FlatResponseDto(f)).ToList();
        }

        public async Task<FlatResponseDto?> GetFlatByAuth0UserIdAsync(string auth0UserId, CancellationToken cancellationToken = default)
        {
            var flat = await _flatRepository.GetByAuth0UserIdAsync(auth0UserId, cancellationToken);
            return flat == null ? null : new FlatResponseDto(flat);
        }

        public async Task<bool> LinkResidentToFlatAsync(string email, string auth0UserId, CancellationToken cancellationToken = default)
        {
            var flat = await _flatRepository.GetByEmailAsync(email, cancellationToken);
            
            // If flat exists and hasn't been linked to an Auth0 user yet
            if (flat != null && string.IsNullOrEmpty(flat.Auth0UserId))
            {
                flat.Auth0UserId = auth0UserId;
                await _flatRepository.UpdateAsync(flat, cancellationToken);
                return true;
            }
            
            return false;
        }
    }
}