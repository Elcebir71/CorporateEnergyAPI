using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CorporateEnergyAPI.Data;
using CorporateEnergyAPI.Models;

namespace CorporateEnergyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndustrialReadingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public IndustrialReadingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<IndustrialReading>>> GetReadings()
        {
            return await _context.IndustrialReadings.ToListAsync();
        }
       
        [HttpPost]
        public async Task<ActionResult<IndustrialReading>> PostReading(IndustrialReading reading)
        {
            _context.IndustrialReadings.Add(reading);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetReadings), new { id = reading.Id }, reading);
        }
    }
}