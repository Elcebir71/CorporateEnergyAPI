using CorporateEnergyAPI.Models;

namespace CorporateEnergyAPI.Interfaces
{
    public interface IIndustrialReadingRepository
    {
        Task<IEnumerable<IndustrialReading>> GetAllReadingsAsync();
    }
}
