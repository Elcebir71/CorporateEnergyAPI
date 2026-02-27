using CorporateEnergyAPI.Models;
namespace CorporateEnergyAPI.Interfaces
{
    public interface IEnergyService
    {
        Task<EnergyDashboardViewModel> GetDashboardSummaryAsync();
        Task<EnergyDashboardViewModel> GetSimulationStepAsync(int offset);
    }
}
