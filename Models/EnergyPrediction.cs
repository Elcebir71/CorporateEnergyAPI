namespace CorporateEnergyAPI.Models
{
    public class EnergyPrediction
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string ModelName { get; set; } = string.Empty;
        public double PredictedPrice { get; set; }
        public double ActualPrice { get; set; }
        public bool IsBestModel { get; set; }
    }
}
