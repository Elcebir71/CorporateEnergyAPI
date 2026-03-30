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
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<IEnumerable<EnergyModel>> GetEnergyMarketDataAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var sql = @"SELECT ""Id"", ""Timestamp"", ""Price_MWh"", ""Is_Green_Energy"", ""System_Code""
                FROM ""EuropeanEnergyData""
                ORDER BY ""Timestamp"" DESC
                LIMIT 30";
            var rows = await connection.QueryAsync<dynamic>(sql);
            return rows.Select(r => new EnergyModel
            {
                Id = Convert.ToInt32(r.Id),
                Timestamp = Convert.ToDateTime(r.Timestamp),
                Price_MWh = Convert.ToDouble(r.Price_MWh),
                Is_Green_Energy = Convert.ToInt32(r.Is_Green_Energy),
                System_Code = (string)r.System_Code
            });
        }

        public async Task<IEnumerable<EnergyModel>> GetSimulationStepAsync(int offset)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var sql = @"SELECT ""Id"", ""Timestamp"", ""Price_MWh"", ""Is_Green_Energy"", ""System_Code""
                FROM ""EuropeanEnergyData""
                ORDER BY ""Timestamp"" DESC
                LIMIT 20 OFFSET @Offset";
            var rows = await connection.QueryAsync<dynamic>(sql, new { Offset = offset });
            return rows.Select(r => new EnergyModel
            {
                Id = Convert.ToInt32(r.Id),
                Timestamp = Convert.ToDateTime(r.Timestamp),
                Price_MWh = Convert.ToDouble(r.Price_MWh),
                Is_Green_Energy = Convert.ToInt32(r.Is_Green_Energy),
                System_Code = (string)r.System_Code
            });
        }

        public async Task<IEnumerable<EnergyPrediction>> GetLatestPredictionsAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var sql = @"SELECT ""Id"", ""Timestamp"", ""ModelName"", ""PredictedPrice"", ""ActualPrice"", ""IsBestModel""
                FROM ""EnergyPredictions""
                ORDER BY ""Timestamp"" DESC
                LIMIT 5";
            var rows = await connection.QueryAsync<dynamic>(sql);
            return rows.Select(r => new EnergyPrediction
            {
                Id = Convert.ToInt32(r.Id),
                Timestamp = Convert.ToDateTime(r.Timestamp),
                ModelName = (string)r.ModelName,
                PredictedPrice = Convert.ToDouble(r.PredictedPrice),
                ActualPrice = Convert.ToDouble(r.ActualPrice),
                IsBestModel = Convert.ToBoolean(r.IsBestModel)
            });
        }
    }
}
