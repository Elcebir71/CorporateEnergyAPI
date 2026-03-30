using Npgsql;
using CorporateEnergyAPI.Interfaces;
using CorporateEnergyAPI.Models;

using Dapper;

namespace CorporateEnergyAPI.Repositories
{
    public class EnergyRepository : IEnergyRepository
    {
        private readonly string _connectionString;

        public EnergyRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Standaard dashboard data (Standart dashboard verisi)
        public async Task<IEnumerable<EnergyModel>> GetEnergyMarketDataAsync()
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);

                // Let op: Verwijder CAST(AS DATE) om uren te behouden voor de simulatie

                var sql = @"
                SELECT Id, Timestamp, Price_MWh, Is_Green_Energy, System_Code
                FROM ""EuropeanEnergyData""
                ORDER BY Timestamp DESC
                LIMIT 30";

                return await connection.QueryAsync<EnergyModel>(sql);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EnergyRepository ERROR: {ex.Message}");
                return Enumerable.Empty<EnergyModel>();
            }
        }

        // Nieuwe methode voor de live simulatie 
        public async Task<IEnumerable<EnergyModel>> GetSimulationStepAsync(int offset)
        {
            using var connection = new NpgsqlConnection(_connectionString);

            // Gebruik OFFSET en FETCH om door de 1.8M rijen te navigeren           
            var sql = @"
                SELECT Id, Timestamp, Price_MWh, Is_Green_Energy, System_Code
                FROM ""EuropeanEnergyData""
                ORDER BY Timestamp ASC
                OFFSET @Offset ROWS FETCH NEXT 20 ROWS ONLY";

            return await connection.QueryAsync<EnergyModel>(sql, new { Offset = offset });
        }
    }
}
