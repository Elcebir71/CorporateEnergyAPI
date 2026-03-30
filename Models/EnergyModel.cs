namespace CorporateEnergyAPI.Models
{
    public class EnergyModel
    {

        public int Id { get; set; }
        public DateTime Timestamp { get; set; }

        // De energieprijs per Megawattuur (MWh başına enerji fiyatı)
        public double Price_MWh { get; set; }

        // Indicator voor groene energie: 1 (Ja) of 0 (Nee)
        // SQL'de 1/0 olarak göründüğü için int veya bool tercih edilebilir
        public int Is_Green_Energy { get; set; }

        // De landcode of systeemcode (Ülke veya sistem kodu, örn: 'HU')
        public string System_Code { get; set; } = string.Empty;
    }
}