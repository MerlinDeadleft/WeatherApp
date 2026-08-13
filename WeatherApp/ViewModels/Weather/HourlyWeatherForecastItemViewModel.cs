using WeatherApp.Core;
using WeatherApp.Models;
using WeatherApp.Services;

namespace WeatherApp.ViewModels.Weather;

public class HourlyWeatherForecastItemViewModel : ViewModelBase, IDisposable
{
    private HourlyConditions hourlyConditions;
    private ISettingsService settingsService;
    private bool useMetric;

    public string ForecastTime => GetForecastTime();
    public string WeatherIconUrl => GetWeatherIconUrl();
    public string Temperature => GetTemperature();
    public string WeatherDescription => GetWeatherDescription();
    public string FeelsLikeTemperature => GetFeelsLikeTemperature();
    public string Humidity => GetHumidity();
    public string Precipitation => GetPrecipitation();
    public string ChanceOfRain => GetChanceOfRain();
    public string WindSpeedAndDirection => GetWindSpeedAndDirection();
    public int WindDirectionArrowRotation => GetWinDirectionArrowRotation();

    public HourlyWeatherForecastItemViewModel(HourlyConditions hourlyConditions, ISettingsService settingsService)
    {
        this.settingsService = settingsService;
        this.settingsService.OnSettingsUpdated += HandleSettingsUpdated;
        useMetric = settingsService.GetSettings().UseMetric;
        this.hourlyConditions = hourlyConditions;
    }
    
    public void Dispose()
    {
        settingsService.OnSettingsUpdated -= HandleSettingsUpdated;
    }

    private string GetForecastTime()
    {
        var hour = int.Parse(hourlyConditions.Time) / 100;
        var timeOfDay = TimeSpan.FromHours(hour);
        var forecastTime = DateTime.Today.Add(timeOfDay);
        return forecastTime.ToString("t");
    }

    private string GetWeatherIconUrl()
    {
        return hourlyConditions.WeatherIconUrl[0].Value;
    }

    private string GetTemperature()
    {
        return useMetric
            ? $"{hourlyConditions.TempC}°C"
            : $"{hourlyConditions.TempF}°F";
    }

    private string GetWeatherDescription()
    {
        return hourlyConditions.WeatherDesc[0].Value;
    }

    private string GetFeelsLikeTemperature()
    {
        return useMetric
            ? $"Feels like: {hourlyConditions.FeelsLikeC}°C"
            : $"Feels like: {hourlyConditions.FeelsLikeF}°F";
    }

    private string GetHumidity()
    {
        return $"Humidity: {hourlyConditions.Humidity}%";
    }

    private string GetPrecipitation()
    {
        return useMetric
            ? $"Precipitation: {hourlyConditions.PrecipMM} mm"
            : $"Precipitation: {hourlyConditions.PrecipInches} in";
    }

    private string GetChanceOfRain()
    {
        return $"Chance of rain: {hourlyConditions.ChanceOfRain}%";
    }

    private string GetWindSpeedAndDirection()
    {
        return useMetric
            ? $"{hourlyConditions.WindSpeedKmph}-{hourlyConditions.WindGustKmph} km/h {hourlyConditions.WindDir16Point}"
            : $"{hourlyConditions.WindSpeedMiles}-{hourlyConditions.WindGustMiles} mph {hourlyConditions.WindDir16Point}";
    }

    private int GetWinDirectionArrowRotation()
    {
        // Add 180 as arrow image points up by default. Arrow would otherwise point in the direction
        // wind comes from instead of where it's going
        return hourlyConditions.WindDirDegree + 180;
    }

    private void HandleSettingsUpdated()
    {
        useMetric = settingsService.GetSettings().UseMetric;
        DispatchPropertyChanged(null);
    }
}