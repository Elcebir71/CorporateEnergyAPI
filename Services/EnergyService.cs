using CorporateEnergyAPI.Interfaces;
using CorporateEnergyAPI.Models;

namespace CorporateEnergyAPI.Services
{
    /**
     * Implementatie van de Energy Service. 
     * Gebruikt de repository voor data-extractie en simulatieverwerking.
     */
    public class EnergyService : IEnergyService
    {
        private readonly IIndustrialReadingRepository _repository;
        private readonly IEnergyRepository _energyRepository;

        public EnergyService(IIndustrialReadingRepository repository, IEnergyRepository energyRepository)
        {
            _repository = repository;
            _energyRepository = energyRepository;
        }

        public async Task<EnergyDashboardViewModel> GetDashboardSummaryAsync()
        {
            // Haal alle industriële gegevens en marktprijzen op (Tüm endüstriyel verileri ve piyasa fiyatlarını çek)
            var readings = await _repository.GetAllReadingsAsync();
            var energyData = await _energyRepository.GetEnergyMarketDataAsync();
            var allReadingsList = readings.ToList();

            var thirtyDaysAgo = DateTime.Now.AddDays(-30);
            var last30DaysReadings = allReadingsList
                .Where(r => r.Timestamp >= thirtyDaysAgo)
                .OrderBy(r => r.Timestamp)
                .ToList();

            return new EnergyDashboardViewModel
            {
                AllReadings = new List<IndustrialReading>(last30DaysReadings),
                TableReadings = new List<IndustrialReading>(allReadingsList
                    .OrderByDescending(r => r.Timestamp)
                    .Take(5)),
                AverageUsage = allReadingsList.Count > 0 ? allReadingsList.Average(r => r.Value) : 0,
                PeakUsage = allReadingsList.Count > 0 ? allReadingsList.Max(r => r.Value) : 0,
                LatestEnergyPrices = new List<EnergyModel>(energyData)
            };
        }

        /**
         * Haalt een specifiek frame van marktgegevens op voor de live simulatie.
         */
        public async Task<EnergyDashboardViewModel> GetSimulationStepAsync(int offset)
        {
            // Haal gegevens op uit de repository met de opgegeven offset
            var simulatedPrices = await _energyRepository.GetSimulationStepAsync(offset);

            // Retourneer een ViewModel dat specifiek is geformatteerd voor de simulatie-UI
            
            return new EnergyDashboardViewModel
            {
                LatestEnergyPrices = new List<EnergyModel>(simulatedPrices)
                // Berekeningen kunnen hier indien nodig worden toegevoegd
            };
        }
    }
}