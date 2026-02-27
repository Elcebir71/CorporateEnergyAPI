namespace CorporateEnergyAPI.Models;

    public class EnergyDashboardViewModel
    {
        public List<IndustrialReading> AllReadings { get; set; } = new();
        public List<IndustrialReading> TableReadings { get; set; } = new();
        public List<EnergyModel> LatestEnergyPrices { get; set; } = new(); // Tablo için son 5

        // Het berekende gemiddelde verbruik
        public double AverageUsage { get; set; }

        // De hoogst gemeten piekwaarde
        public double PeakUsage { get; set; }

    }