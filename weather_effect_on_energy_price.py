import xarray as xr
import pandas as pd
import numpy as np
import os

base_path = r"C:\Users\HakanSahin\Downloads\60517c1f6902130e146f45286136b63f (1)"
files = [
    "data_stream-oper_stepType-accum.nc",
    ]
file = os.path.join(base_path, files[0])
basepath=r"C:\Users\HakanSahin\source\repos\CorporateEnergyAPI"
file_name = os.path.join(basepath,'weather_fixed.csv')
df_daily_weather = pd.read_csv(file_name)
ds_solar = xr.open_dataset(file)
df_solar_daily = ds_solar['ssrd'].mean(dim=['latitude','longitude']).to_dataframe().reset_index()
df_solar_daily['date'] = df_solar_daily['valid_time'].dt.date
df_daily_weather['time'] = pd.to_datetime(df_daily_weather['time'], format='mixed')
df_daily_weather = df_daily_weather.groupby(df_daily_weather['time'].dt.date)[['Temperature_C', 'WindSpeed_ms']].mean().reset_index()
df_daily_weather.columns = ['date', 'Avg_Temp', 'Avg_Wind']
df_solar_final = df_solar_daily.groupby('date')['ssrd'].mean().reset_index()
df_solar_final.columns=['date','Avg_SolarRadiation']    

print("Hava durumu sütunları:", df_daily_weather.columns)
print("Güneş verisi sütunları:", df_solar_final.columns)
df_final_merged = pd.merge(df_daily_weather, df_solar_final, on='date', how='inner')

print(df_final_merged.head())