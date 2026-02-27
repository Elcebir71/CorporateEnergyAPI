using CorporateEnergyAPI.Models;
namespace CorporateEnergyAPI.Interfaces
{
    public interface IEnergyRepository
    {
        Task<IEnumerable<EnergyModel>> GetEnergyMarketDataAsync();
        Task<IEnumerable<EnergyModel>> GetSimulationStepAsync(int offset);
    }
}
