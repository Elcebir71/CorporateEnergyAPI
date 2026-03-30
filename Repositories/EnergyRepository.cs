using Npgsql;
﻿using CorporateEnergyAPI.Interfaces;
using CorporateEnergyAPI.Models;

using Dapper;

namespace CorporateEnergyAPI.Repositories
{
    public class EnergyRepository : IEnergyRepository
    {
        private readonly string _connectionString = "Server=localhost,1444;Database=CorporateEnergyDb;User Id=sa;Password=NjKrK1825!;TrustServerCertificate=True;";

        // Standaard dashboard data (Standart dashboard verisi)
        public async Task<IEnumerable<EnergyModel>> GetEnergyMarketDataAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);

            // Let op: Verwijder CAST(AS DATE) om uren te behouden voor de simulatie
          
            var sql = @"
                SELECT TOP 30
                    Timestamp, 
                    Price_MWh,
                    Is_Green_Energy,
                    System_Code
                FROM EuropeanEnergyData
                ORDER BY Timestamp DESC";

            return await connection.QueryAsync<EnergyModel>(sql);
        }

        // Nieuwe methode voor de live simulatie 
        public async Task<IEnumerable<EnergyModel>> GetSimulationStepAsync(int offset)
        {
            using var connection = new NpgsqlConnection(_connectionString);

            // Gebruik OFFSET en FETCH om door de 1.8M rijen te navigeren           
            var sql = @"
                SELECT 
                    Timestamp, 
                    Price_MWh, 
                    Is_Green_Energy, 
                    System_Code
                FROM EuropeanEnergyData
                ORDER BY Timestamp ASC 
                OFFSET @Offset ROWS FETCH NEXT 20 ROWS ONLY";

            return await connection.QueryAsync<EnergyModel>(sql, new { Offset = offset });
        }
    }
}
