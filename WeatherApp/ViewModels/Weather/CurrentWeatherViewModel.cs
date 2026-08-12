using WeatherApp.Core;
using WeatherApp.Models;
using WeatherApp.Services;

namespace WeatherApp.ViewModels.Weather;

public class CurrentWeatherViewModel : ViewModelBase
{
    private readonly ISettingsService settingsService;
    private SettingsModel settingsModel;
    private WeatherModel? weatherModel;

    public string LocationName => GetLocationName();
    public string LocationData => GetLocationData();
    public string WeatherIconUrl => GetWeatherIconUrl();
    public string Temperature => GetTemperature();
    public string Description => GetWeatherDescription();
    public string FeelsLikeTemperature => GetFeelsLikeTemperature();
    public string Humidity => GetHumidity();
    public string Precipitation => GetPrecipitation();
    public string WindSpeedAndDirection => GetWindSpeedAndDirection();
    public int WindDirectionArrowRotation => GetWindDirectionArrowRotation();

    public CurrentWeatherViewModel(ISettingsService settingsService)
    {
        this.settingsService = settingsService;
        this.settingsService.OnSettingsUpdated += HandleSettingsUpdated;
        settingsModel = this.settingsService.LoadSettings();
    }

    public void UpdateWeatherModel(WeatherModel? weatherModel)
    {
        this.weatherModel = weatherModel;
        DispatchPropertyChanged(null);
    }

    private string GetLocationName()
    {
        if(weatherModel == null)
            return "Loading weather data...";

        if(weatherModel.Data == null)
            return $"No weather data for {weatherModel.LocationName} available or invalid location";

        return $"{weatherModel.LocationName}";
    }

    private string GetLocationData()
    {
        if(weatherModel == null || weatherModel.Data == null)
            return "";

        var locationData = weatherModel.Data.Value.LocationData[0];
        return $"{locationData.AreaName[0].Value}, {locationData.Region[0].Value}, {locationData.Country[0].Value}";
    }

    private string GetWeatherIconUrl()
    {
        if(weatherModel == null || weatherModel.Data == null)
            return "";

        return weatherModel.Data.Value.CurrentCondition[0].WeatherIconUrl[0].Value;
    }

    private string GetTemperature()
    {
        if(weatherModel == null || weatherModel.Data == null)
            return "";

        return settingsModel.UseMetric
            ? $"{weatherModel.Data.Value.CurrentCondition[0].TempC}°C"
            : $"{weatherModel.Data.Value.CurrentCondition[0].TempF}°F";
    }

    private string GetWeatherDescription()
    {
        if(weatherModel == null || weatherModel.Data == null)
            return "";

        return $"{weatherModel.Data.Value.CurrentCondition[0].WeatherDesc[0].Value}";
    }

    private string GetFeelsLikeTemperature()
    {
        if(weatherModel == null || weatherModel.Data == null)
            return "";

        var temperature = settingsModel.UseMetric
            ? $"{weatherModel.Data.Value.CurrentCondition[0].FeelsLikeC}°C"
            : $"{weatherModel.Data.Value.CurrentCondition[0].FeelsLikeF}°F";

        return $"Feels like {temperature}";
    }

    private string GetHumidity()
    {
        if(weatherModel == null || weatherModel.Data == null)
            return "";

        return $"Humidity: {weatherModel.Data.Value.CurrentCondition[0].Humidity}%";
    }

    private string GetPrecipitation()
    {
        if(weatherModel == null || weatherModel.Data == null)
            return "";

        var precipitation = settingsModel.UseMetric
            ? $"{weatherModel.Data.Value.CurrentCondition[0].PrecipMM:0.0#} mm"
            : $"{weatherModel.Data.Value.CurrentCondition[0].PrecipInches:0.0#} in";
            
        return $"Precipitation: {precipitation}";
    }

    private string GetWindSpeedAndDirection()
    {
        if(weatherModel == null || weatherModel.Data == null)
            return "";

        var windSpeed = settingsModel.UseMetric
            ? $"{weatherModel.Data.Value.CurrentCondition[0].WindSpeedKmph} km/h"
            : $"{weatherModel.Data.Value.CurrentCondition[0].WindSpeedMiles} mph";
        
        return $"{windSpeed} {weatherModel.Data.Value.CurrentCondition[0].WindDir16Point}";
    }

    private int GetWindDirectionArrowRotation()
    {
        if(weatherModel == null || weatherModel.Data == null)
            return 0;

        // Add 180 as arrow image points up by default. Arrow would otherwise point in the direction
        // wind comes from instead of where it's going
        return weatherModel.Data.Value.CurrentCondition[0].WindDirDegree + 180;
    }

    private void HandleSettingsUpdated()
    {
        settingsModel = settingsService.LoadSettings();
        DispatchPropertyChanged(null);
    }
}