using Microsoft.EntityFrameworkCore;
using SocietyBill.Application.Interfaces.Repositories;
using SocietyBill.Domain.Entities;
using SocietyBill.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SocietyBill.Infrastructure.Repositories
{
    public class BillRepository : IBillRepository
    {
        private readonly AppDbContext _context;
        public BillRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Bill?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Bills.Include(b => b.Flat).FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        }

        public async Task<List<Bill>> GetByFlatIdAsync(Guid flatId, int year, CancellationToken cancellationToken = default)
        {
            return await _context.Bills.AsNoTracking()
                .Include(b => b.Flat)
                .Where(b => b.FlatId == flatId && b.Year == year)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Bill>> GetBySocietyAsync(Guid societyId, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _context.Bills.AsNoTracking()
                .Include(b => b.Flat)
                .Where(b => b.Flat.SocietyId == societyId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Bill bill, CancellationToken cancellationToken = default)
        {
            await _context.Bills.AddAsync(bill, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Bill bill, CancellationToken cancellationToken = default)
        {
            _context.Bills.Update(bill);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> GetOutstandingAmountAsync(Guid flatId, CancellationToken cancellationToken = default)
        {
            return await _context.Bills.AsNoTracking()
                .Where(b => b.FlatId == flatId && !b.IsPaid)
                .SumAsync(b => (int)b.Amount, cancellationToken);
        }
    }
}