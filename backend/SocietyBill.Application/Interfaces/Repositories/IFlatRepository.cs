using SocietyBill.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SocietyBill.Application.Interfaces.Repositories
{
    public interface IFlatRepository
    {
        Task<Flat?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Flat>> GetBySocietyAsync(Guid societyId, int page, int pageSize, CancellationToken cancellationToken = default);
        Task<Flat?> GetByAuth0UserIdAsync(string auth0UserId, CancellationToken cancellationToken = default);
        Task<Flat?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task AddAsync(Flat flat, CancellationToken cancellationToken = default);
        Task UpdateAsync(Flat flat, CancellationToken cancellationToken = default);
    }
}