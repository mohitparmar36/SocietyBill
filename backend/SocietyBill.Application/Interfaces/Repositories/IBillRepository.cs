using SocietyBill.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SocietyBill.Application.Interfaces.Repositories
{
    public interface IBillRepository
    {
        Task<Bill?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Bill>> GetByFlatIdAsync(Guid flatId, int year, CancellationToken cancellationToken = default);
        Task<List<Bill>> GetBySocietyAsync(Guid societyId, int page, int pageSize, CancellationToken cancellationToken = default);
        Task AddAsync(Bill bill, CancellationToken cancellationToken = default);
        Task UpdateAsync(Bill bill, CancellationToken cancellationToken = default);
        Task<int> GetOutstandingAmountAsync(Guid flatId, CancellationToken cancellationToken = default);
    }
}