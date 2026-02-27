using CorporateEnergyAPI.Models;
namespace CorporateEnergyAPI.Interfaces
{
    public interface IWeatherRepository
    {
        Task<List<WeatherData>> GetAllAsync();
        Task AddRangeAsync(List<WeatherData> data);
    }
}
