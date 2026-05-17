using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SocietyBill.Domain.Entities;

namespace SocietyBill.Infrastructure.Data
{
    public class DbSeeder
    {
        private readonly AppDbContext _context;
        public DbSeeder(AppDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            if (!await _context.Societies.AnyAsync())
            {
                var society = new Society
                {
                    Id = Guid.NewGuid(),
                    Name = "Sample Society",
                    Address = "123 Main St",
                    CreatedAt = DateTime.UtcNow
                };
                _context.Societies.Add(society);
                await _context.SaveChangesAsync();
            }
        }
    }
}