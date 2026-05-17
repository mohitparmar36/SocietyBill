using SocietyBill.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SocietyBill.Application.Interfaces.Repositories
{
    public interface ISocietyRepository
    {
        Task<Society?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddAsync(Society society, CancellationToken cancellationToken = default);
        Task UpdateAsync(Society society, CancellationToken cancellationToken = default);
    }
}