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
    public class FlatRepository : IFlatRepository
    {
        private readonly AppDbContext _context;
        public FlatRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Flat?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Flats.Include(f => f.Bills).FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
        }

        public async Task<List<Flat>> GetBySocietyAsync(Guid societyId, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _context.Flats.AsNoTracking()
                .Where(f => f.SocietyId == societyId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<Flat?> GetByAuth0UserIdAsync(string auth0UserId, CancellationToken cancellationToken = default)
        {
            return await _context.Flats.Include(f => f.Bills)
                .FirstOrDefaultAsync(f => f.Auth0UserId == auth0UserId, cancellationToken);
        }

        public async Task<Flat?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Flats.Include(f => f.Bills)
                .FirstOrDefaultAsync(f => f.Email == email, cancellationToken);
        }

        public async Task AddAsync(Flat flat, CancellationToken cancellationToken = default)
        {
            await _context.Flats.AddAsync(flat, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Flat flat, CancellationToken cancellationToken = default)
        {
            _context.Flats.Update(flat);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}