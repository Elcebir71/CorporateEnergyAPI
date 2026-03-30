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
            var energyList = energyData.ToList();
            var predictions = await _energyRepository.GetLatestPredictionsAsync();

            var thirtyDaysAgo = DateTime.Now.AddDays(-30);
            var last30DaysReadings = readings
                .Where(r => r.Timestamp >= thirtyDaysAgo)
                .OrderBy(r => r.Timestamp)
                .ToList();

            return new EnergyDashboardViewModel
            {
                AllReadings = readings.ToList(),
                TableReadings = readings.OrderByDescending(r => r.Timestamp).Take(5).ToList(),
                AverageUsage = energyList.Count > 0 ? energyList.Average(r => r.Price_MWh) : 0,
                PeakUsage = energyList.Count > 0 ? energyList.Max(r => r.Price_MWh) : 0,
                LatestEnergyPrices = energyList,
                Predictions = predictions.ToList()
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
        public async Task<IEnumerable<EnergyPrediction>> GetLatestPredictionsAsync()
        {
            return await _energyRepository.GetLatestPredictionsAsync();
        }
    }
}