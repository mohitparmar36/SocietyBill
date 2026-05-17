using Microsoft.EntityFrameworkCore;
using SocietyBill.Application.Interfaces.Repositories;
using SocietyBill.Domain.Entities;
using SocietyBill.Infrastructure.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SocietyBill.Infrastructure.Repositories
{
    public class SocietyRepository : ISocietyRepository
    {
        private readonly AppDbContext _context;
        public SocietyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Society?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Societies.Include(s => s.Flats).FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task AddAsync(Society society, CancellationToken cancellationToken = default)
        {
            await _context.Societies.AddAsync(society, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Society society, CancellationToken cancellationToken = default)
        {
            _context.Societies.Update(society);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}