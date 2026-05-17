using SocietyBill.Application.DTOs.Request;
using SocietyBill.Application.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SocietyBill.Application.Interfaces.Services
{
    public interface IFlatService
    {
        Task<FlatResponseDto> CreateFlatAsync(FlatCreateRequestDto request, CancellationToken cancellationToken = default);
        Task<List<FlatResponseDto>> GetFlatsForSocietyAsync(int page, int pageSize, CancellationToken cancellationToken = default);
        Task<FlatResponseDto?> GetFlatByAuth0UserIdAsync(string auth0UserId, CancellationToken cancellationToken = default);
        Task<bool> LinkResidentToFlatAsync(string email, string auth0UserId, CancellationToken cancellationToken = default);
    }
}