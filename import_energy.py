import pandas as pd
import psycopg2
from psycopg2.extras import execute_batch

conn = psycopg2.connect(
    host="localhost",
    port=5432,
    database="railway",
    user="postgres",
    password="postgres"
)

df = pd.read_csv("energy_dataset.csv", usecols=["time", "price actual"])
df = df.dropna(subset=["price actual"])
df.columns = ["Timestamp", "Price_MWh"]
df["Is_Green_Energy"] = 0
df["System_Code"] = "ES"
df["Timestamp"] = pd.to_datetime(df["Timestamp"], utc=True)

cursor = conn.cursor()
cursor.execute("""
    CREATE TABLE IF NOT EXISTS "EuropeanEnergyData" (
        "Id" SERIAL PRIMARY KEY,
        "Timestamp" TIMESTAMPTZ,
        "Price_MWh" DOUBLE PRECISION,
        "Is_Green_Energy" INTEGER,
        "System_Code" VARCHAR(10)
    )
""")

data = list(df[["Timestamp","Price_MWh","Is_Green_Energy","System_Code"]].itertuples(index=False, name=None))
execute_batch(cursor, """
    INSERT INTO "EuropeanEnergyData" ("Timestamp", "Price_MWh", "Is_Green_Energy", "System_Code")
    VALUES (%s, %s, %s, %s)
""", data)

conn.commit()
cursor.close()
conn.close()
print(f"Import edildi: {len(data)} satır")
