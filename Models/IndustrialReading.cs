namespace CorporateEnergyAPI.Models
{
    public class IndustrialReading
    {
        public int Id { get; set; }
        public string SensorName { get; set; } = string.Empty;
        public double Value { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}