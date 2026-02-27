import kagglehub
import pandas as pd
import numpy as np
from sklearn.ensemble import RandomForestRegressor
from sqlalchemy import create_engine, DateTime, Float, Integer
import urllib
import json, os
import pyodbc
from datetime import datetime, timedelta
import random

params = urllib.parse.quote_plus(
    "DRIVER={ODBC Driver 17 for SQL Server};"
    "SERVER=localhost,1444;"
    "DATABASE=CorporateEnergyDb;"
    "UID=sa;"
    "PWD=NjKrK1825!;"
    "TrustServerCertificate=yes"
)

def clean_timestamp(df):
    """
    Zorgt voor de juiste berekening van tijdstempels, inclusief de correctie voor 24:00 uur.
    """
    # Identificeer rijen waar het uur 24 is
    mask = df['hora'] == 24
    
    # Zet 24:00 om naar 00:00
    df.loc[mask, 'hora'] = 0
    
    # Maak een volledige Timestamp kolom door datum en tijd samen te voegen
    df['Timestamp'] = pd.to_datetime(df['fecha'] + ' ' + df['hora'].astype(str) + ':00:00')

    # Voeg een dag toe aan de tijdstempels die oorspronkelijk 24:00 waren
    df.loc[mask, 'Timestamp'] += td(days=1)

    return df

# Verbindingsparameters voor de SQL Server Docker container

# Initialiseer de database engine 
engine = create_engine(f"mssql+pyodbc:///?odbc_connect={params}")

def get_data_from_sql():
    """Haalt ruwe industriële gegevens op voor statistische analyse."""
    query = "SELECT * FROM IndustrialReadings"
    return pd.read_sql(query, engine)

def save_energy_data_to_sql(df):
    """
    Slaat de verwerkte marktgegevens op in SQL Server met expliciete gegevenstypen.
    """
    try:
        # Definieer SQL-typen om ervoor te zorgen dat tijdstempels uren bevatten
        dtype_mapping = {
            'Timestamp': DateTime(),
            'Price_MWh': Float(),
            'Is_Green_Energy': Integer()
        }

        # Sla de gegevens op in de tabel 'EuropeanEnergyData'
        df.to_sql('EuropeanEnergyData', engine, if_exists='replace', index=False, dtype=dtype_mapping)
        print("✅ Succes: Gegevens met volledige tijdstempels zijn opgeslagen.")
        return True
    except Exception as e:
        print(f"❌ Fout bij het opslaan in de database: {e}")
        return False

def perform_statistical_analysis(df):
    """Berekent het gemiddelde verbruik en de piekbelasting."""
    if df.empty: return {"average": 0, "peak": 0}
    avg_usage = df['Value'].mean()
    max_usage = df['Value'].max()
    return {"average": round(avg_usage, 2), "peak": round(max_usage, 2)}

def process_energy_data():
    """
    Hoofdfunctie voor het downloaden, transformeren en uploaden van de marktgegevens.
    """
    # Download de nieuwste dataset van Kaggle
    path = kagglehub.dataset_download("pythonafroz/european-union-energy-market-data")
    csv_files = [f for f in os.listdir(path) if f.endswith('.csv')][0]
    full_path = os.path.join(path, csv_files)

    # Laad de ruwe CSV-gegevens
    df = pd.read_csv(full_path)
    
    # Corrigeer de tijdstempels 
    df = clean_timestamp(df)

    # Hernoem kolommen naar de namen die in het C# model worden gebruikt
    df_clean = df.rename(columns={
            'precio': 'Price_MWh',
            'bandera': 'Is_Green_Energy',
            'sistema': 'System_Code'
        })

    # Selecteer alleen de relevante kolommen voor het dashboard
    final_df = df_clean[['Timestamp', 'Price_MWh', 'Is_Green_Energy', 'System_Code']]
    final_df = final_df.dropna(subset=['Price_MWh'])

    # Upload de uiteindelijke resultaten naar de SQL-database 
    save_energy_data_to_sql(final_df)

# Database connectie configuratie
CONN_STR = "DRIVER={ODBC Driver 17 for SQL Server};SERVER=localhost,1444;DATABASE=CorporateEnergyDb;UID=sa;PWD=NjKrK1825!"

def check_date_range():
    """Controleer het datumbereik van de bestaande gegevens."""
    conn = pyodbc.connect(CONN_STR)
    cursor = conn.cursor()
    
    cursor.execute("""
        SELECT 
            MIN(Timestamp) as EersteDatum,
            MAX(Timestamp) as LaatsteDatum,
            COUNT(*) as TotaalRecords,
            COUNT(DISTINCT CAST(Timestamp AS DATE)) as AantalDagen
        FROM EuropeanEnergyData
    """)
    
    row = cursor.fetchone()
    print(f"Eerste datum: {row.EersteDatum}")
    print(f"Laatste datum: {row.LaatsteDatum}")
    print(f"Totaal records: {row.TotaalRecords}")
    print(f"Aantal dagen: {row.AantalDagen}")
    
    conn.close()

def generate_30_days_data():
    """Genereer 30 dagen aan testgegevens voor de energiemarkt."""
    conn = pyodbc.connect(CONN_STR)
    cursor = conn.cursor()
    
    # Verwijder oude testgegevens (optioneel)
    # cursor.execute("DELETE FROM EuropeanEnergyData")
    
    system_codes = ['NL', 'DE', 'BE', 'FR', 'HU']
    
    # Genereer gegevens voor de laatste 30 dagen
    for day in range(30):
        for hour in range(0, 24, 4):  # Elke 4 uur een meting
            timestamp = datetime.now() - timedelta(days=day, hours=hour)
            price = random.uniform(80, 300)
            is_green = random.choice([0, 1])
            system_code = random.choice(system_codes)
            
            cursor.execute("""
                INSERT INTO EuropeanEnergyData (Timestamp, Price_MWh, Is_Green_Energy, System_Code)
                VALUES (?, ?, ?, ?)
            """, timestamp, round(price, 2), is_green, system_code)
    
    conn.commit()
    conn.close()
    print("30 dagen aan gegevens succesvol gegenereerd!")



# Verbindingsparameters 

engine = create_engine(f"mssql+pyodbc:///?odbc_connect={params}")

def train_and_predict():
    """
    Traint een model op historische data en voorspelt de prijs voor het volgende uur.
    (Geçmiş verilerle model eğitir ve bir sonraki saat için fiyat tahmini yapar.)
    """
    # Haal de laatste 10.000 rijen op voor training
    query = "SELECT TOP 10000 Timestamp, Price_MWh FROM EuropeanEnergyData ORDER BY Timestamp DESC"
    df = pd.read_sql(query, engine)
    df = df.sort_values('Timestamp')

    # Feature Engineering (Kenmerken maken)
    df['hour'] = df['Timestamp'].dt.hour
    df['dayofweek'] = df['Timestamp'].dt.dayofweek
    df['prev_price'] = df['Price_MWh'].shift(1) # De prijs van het vorige uur
    df = df.dropna()

    # Model trainen (Random Forest Regressor)
    X = df[['hour', 'dayofweek', 'prev_price']]
    y = df['Price_MWh']
    
    model = RandomForestRegressor(n_estimators=100)
    model.fit(X, y)
    print("✅ Model succesvol getraind.")

    # Voorspel de prijs voor de volgende stap 
    last_price = df['Price_MWh'].iloc[-1]
    next_hour = (df['Timestamp'].iloc[-1].hour + 1) % 24
    next_day = df['Timestamp'].iloc[-1].dayofweek
    
    prediction = model.predict([[next_hour, next_day, last_price]])[0]
    
    return round(prediction, 2)

# De voorspelling opslaan in een nieuwe tabel 'EnergyPredictions'

if __name__ == "__main__":
    # Voer de analyse van industriële gegevens uit 
    try:
        industrial_df = get_data_from_sql()
        stats = perform_statistical_analysis(industrial_df)
        print(f"Analyse Voltooid: Gemiddelde = {stats['average']} kW")
    except Exception:
        print("Geen industriële gegevens beschikbaar voor analyse.")

    # Start het proces voor Europese marktgegevens 
    process_energy_data()

    print("=== Huidige gegevens controleren ===")
    check_date_range()
    
    print("\n=== Nieuwe gegevens genereren? ===")
    answer = input("Wilt u 30 dagen aan testgegevens genereren? (ja/nee): ")
    if answer.lower() == "ja":
        generate_30_days_data()
        print("\n=== Nieuwe gegevens gecontroleerd ===")
        check_date_range()