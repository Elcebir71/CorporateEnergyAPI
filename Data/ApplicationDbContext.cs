using Microsoft.EntityFrameworkCore;
using CorporateEnergyAPI.Models;

namespace CorporateEnergyAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        
        public DbSet<IndustrialReading> IndustrialReadings { get; set; }
    }
}