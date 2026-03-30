import pandas as pd
import numpy as np
from sklearn.ensemble import RandomForestRegressor, GradientBoostingRegressor
from sklearn.linear_model import LinearRegression
from sklearn.preprocessing import StandardScaler
from sqlalchemy import create_engine, text
from datetime import datetime

engine = create_engine("postgresql+psycopg2://postgres:hpDMYvMroMBWZLkcstUaLdHEUbrTqNao@hopper.proxy.rlwy.net:13202/railway")

def get_data():
    df = pd.read_sql('SELECT "Timestamp", "Price_MWh" FROM "EuropeanEnergyData" ORDER BY "Timestamp" DESC LIMIT 10000', engine)
    df = df.sort_values('Timestamp')
    df['hour'] = df['Timestamp'].dt.hour
    df['dayofweek'] = df['Timestamp'].dt.dayofweek
    df['prev_price'] = df['Price_MWh'].shift(1)
    df['prev_price2'] = df['Price_MWh'].shift(2)
    df['rolling_mean'] = df['Price_MWh'].rolling(24).mean()
    return df.dropna()

def predict_all():
    df = get_data()
    X = df[['hour', 'dayofweek', 'prev_price', 'prev_price2', 'rolling_mean']]
    y = df['Price_MWh']

    last_row = X.iloc[-1]
    actual_price = float(df['Price_MWh'].iloc[-1])

    predictions = {}

    # 1. Random Forest
    rf = RandomForestRegressor(n_estimators=100, random_state=42)
    rf.fit(X, y)
    predictions['RandomForest'] = round(float(rf.predict([last_row])[0]), 2)

    # 2. Gradient Boosting
    gb = GradientBoostingRegressor(n_estimators=100, random_state=42)
    gb.fit(X, y)
    predictions['GradientBoosting'] = round(float(gb.predict([last_row])[0]), 2)

    # 3. Linear Regression
    lr = LinearRegression()
    lr.fit(X, y)
    predictions['LinearRegression'] = round(float(lr.predict([last_row])[0]), 2)

    # 4. Moving Average (24h)
    predictions['MovingAverage'] = round(float(df['Price_MWh'].tail(24).mean()), 2)

    # 5. Weighted Moving Average
    weights = np.arange(1, 25)
    wma = np.average(df['Price_MWh'].tail(24), weights=weights)
    predictions['WeightedMovingAverage'] = round(float(wma), 2)

    # En iyi tahmin (gerçek fiyata en yakın)
    best_model = min(predictions, key=lambda k: abs(predictions[k] - actual_price))

    print(f"Gerçek fiyat: {actual_price} €/MWh")
    for model, pred in predictions.items():
        marker = "⭐" if model == best_model else "  "
        print(f"{marker} {model}: {pred} €/MWh")
    print(f"\nEn iyi model: {best_model}")

    return predictions, best_model, actual_price

def save_predictions(predictions, best_model, actual_price):
    with engine.connect() as conn:
        conn.execute(text("""
            CREATE TABLE IF NOT EXISTS "EnergyPredictions" (
                "Id" SERIAL PRIMARY KEY,
                "Timestamp" TIMESTAMPTZ,
                "ModelName" TEXT,
                "PredictedPrice" DOUBLE PRECISION,
                "ActualPrice" DOUBLE PRECISION,
                "IsBestModel" BOOLEAN
            )
        """))
        for model, pred in predictions.items():
            conn.execute(text("""
                INSERT INTO "EnergyPredictions" ("Timestamp", "ModelName", "PredictedPrice", "ActualPrice", "IsBestModel")
                VALUES (:ts, :model, :pred, :actual, :best)
            """), {
                "ts": datetime.utcnow(),
                "model": model,
                "pred": pred,
                "actual": actual_price,
                "best": model == best_model
            })
        conn.commit()
    print("✅ Tüm tahminler kaydedildi.")

if __name__ == "__main__":
    predictions, best_model, actual_price = predict_all()
    save_predictions(predictions, best_model, actual_price)
