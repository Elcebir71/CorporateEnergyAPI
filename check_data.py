import pyodbc

conn = pyodbc.connect(
    "DRIVER={ODBC Driver 17 for SQL Server};"
    "SERVER=localhost,1444;"
    "DATABASE=CorporateEnergyDb;"
    "UID=sa;PWD=NjKrK1825!"
)
cursor = conn.cursor()

cursor.execute("""
    SELECT 
        MIN(Timestamp) as Ilk,
        MAX(Timestamp) as Son,
        COUNT(*) as Toplam,
        COUNT(DISTINCT CAST(Timestamp AS DATE)) as GunSayisi
    FROM EuropeanEnergyData
""")

row = cursor.fetchone()
print(f"Ilk tarih: {row[0]}")
print(f"Son tarih: {row[1]}")
print(f"Toplam kayit: {row[2]}")
print(f"Gun sayisi: {row[3]}")

conn.close()