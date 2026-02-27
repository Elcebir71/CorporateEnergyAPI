namespace CorporateEnergyAPI.Models
{
    public class WeatherData
    {
        public DateTime Timestamp { get; set; }
        public double Temp_2m { get; set; } = 0;
        public double WindSpeed_10m { get; set; } = 0;
        public double SolarRadiation { get; set; } = 0;
        public double ActualPrice { get; set; } = 0;

    }
}
