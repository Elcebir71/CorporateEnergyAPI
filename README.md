# CorporateEnergyAPI ⚡

A **.NET 10 ASP.NET Core + Blazor Server** project for collecting, storing, and visualizing corporate/industrial energy data.

This repository includes two main parts:
- **REST API** for reading and writing industrial sensor measurements.
- **Dashboard UI** for viewing recent readings, usage KPIs, and trend charts.

## Features

- Store industrial energy readings in SQL Server (Entity Framework Core).
- Read/write measurements via `api/IndustrialReading`.
- Blazor dashboard with:
  - Average usage
  - Peak usage
  - Status card (critical/stable)
  - Latest readings table
  - Line chart (JS interop)
  - CSV export
- Retrieve market energy price data from `EuropeanEnergyData` using Dapper.

## Tech Stack

- .NET 10 (ASP.NET Core Web)
- Blazor Server Components
- Entity Framework Core (SQL Server)
- Dapper
- Swagger / OpenAPI
- JavaScript (chart + CSV helper)

## Repository Structure

```text
CorporateEnergyAPI/
├── Controllers/                  # API controllers
├── Data/                         # EF Core DbContext
├── Interfaces/                   # Service/repository interfaces
├── Models/                       # Domain + view models
├── Pages/                        # Blazor pages
├── Repositories/                 # Data access layer (EF + Dapper)
├── Services/                     # Business logic
├── Migrations/                   # EF Core migrations
├── wwwroot/                      # Static assets (css/js)
└── Program.cs                    # App startup/configuration
```

## Prerequisites

- .NET SDK 10.0+
- SQL Server (local or remote)
- (Optional) EF Core CLI:
  ```bash
  dotnet tool install --global dotnet-ef
  ```

## Getting Started

### 1) Clone the repository

```bash
git clone <repo-url>
cd CorporateEnergyAPI
```

### 2) Configure database connections

Update connection settings in two places:

1. **EF Core connection** (typically in `appsettings.json` or user-secrets)
2. **Dapper connection** (`_connectionString` in `Repositories/EnergyRepository.cs`)

> Security note: avoid hard-coding credentials in source files for production. Prefer `IConfiguration`, environment variables, and/or secret managers.

### 3) Apply migrations

```bash
dotnet ef database update
```

### 4) Run the app

```bash
dotnet run
```

- Swagger (Development): `https://localhost:<port>/swagger`
- Dashboard: `https://localhost:<port>/`

## API Usage

Base route:

```text
/api/IndustrialReading
```

### GET all readings

```http
GET /api/IndustrialReading
```

Example response:

```json
[
  {
    "id": 1,
    "sensorName": "Boiler-1",
    "value": 482.3,
    "timestamp": "2026-03-29T09:30:00Z"
  }
]
```

### POST a new reading

```http
POST /api/IndustrialReading
Content-Type: application/json
```

Example request:

```json
{
  "sensorName": "Compressor-A",
  "value": 620.75,
  "timestamp": "2026-03-29T10:45:00Z"
}
```

## Dashboard Data Flow

When the dashboard loads:

1. Industrial readings are fetched from the repository.
2. Market energy prices are fetched using Dapper.
3. The service layer prepares the last 30 days of data.
4. Average/peak KPIs and latest table rows are calculated.
5. Chart data is rendered via JavaScript interop.

## Helper Python Scripts

The repo also contains helper scripts for data preparation/analysis:

- `energy_analyzer.py`
- `weather_data_converter.py`
- `weather_effect_on_energy_price.py`
- `check_data.py`

These scripts are optional and independent from the API runtime.

## Development Notes

- Swagger is enabled automatically in Development.
- EF Core migration files are included in the repository.
- `IndustrialReading` model fields:
  - `Id`
  - `SensorName`
  - `Value`
  - `Timestamp`

## Contributing

Pull requests are welcome.

Suggested flow:

1. Create a branch
2. Make your changes
3. Run checks/build
4. Open a PR

## License

This project is licensed under the **MIT License**. See the [LICENSE](./LICENSE) file for details.
