using CorporateEnergyAPI.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- Services Configuration ---

// Voeg ondersteuning toe voor Controllers (API-eindpunten)
builder.Services.AddControllers();

// Configureer de SQL Server verbinding met de connection string uit appsettings.json
// Dit is de 'brug' naar onze database.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Voeg Swagger/OpenAPI toe voor documentatie (handig voor testen)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- HTTP Request Pipeline ---

// Schakel Swagger in als we in de ontwikkelmodus zijn
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Map de controllers zodat de API bereikbaar is
app.MapControllers();

app.Run();
