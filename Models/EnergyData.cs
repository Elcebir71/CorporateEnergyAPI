namespace CorporateEnergyAPI.Models
{
    public class EnergyData
    {
        public DateTime Timestamp { get; set; } // De tijdstempel van de prijs
        public double Price_MWh { get; set; }   // De prijs per Megawattuur
        public string? Is_Green_Energy { get; set; } // Indicator voor groene energie (Y/N)
        public int System_Code { get; set; }    // De systeem-id
    }
}
