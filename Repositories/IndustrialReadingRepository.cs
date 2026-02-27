using CorporateEnergyAPI.Data;
using CorporateEnergyAPI.Interfaces;
using CorporateEnergyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CorporateEnergyAPI.Repositories
{
    public class IndustrialReadingRepository : IIndustrialReadingRepository
    {
        private readonly ApplicationDbContext _context;

        public IndustrialReadingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<IndustrialReading>> GetAllReadingsAsync()
        {
            // Haal alle metingen op, gesorteerd op tijd
            return await _context.IndustrialReadings
                                 .OrderByDescending(x => x.Timestamp)
                                 .ToListAsync();
        }
    }
}
