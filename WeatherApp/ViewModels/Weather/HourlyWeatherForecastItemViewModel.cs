using WeatherApp.Core;
using WeatherApp.Models;

namespace WeatherApp.ViewModels.Weather;

public class HourlyWeatherForecastItemViewModel : ViewModelBase
{
    private HourlyConditions hourlyConditions;

    public string ForecastTime => GetForecastTime();
    public string WeatherIconUrl => GetWeatherIconUrl();
    public string Temperature => GetTemperature();
    public string WeatherDescription => GetWeatherDescription();
    public string FeelsLikeTemperature => GetFeelsLikeTemperature();
    public string Humidity => GetHumidity();
    public string Precipitation => GetPrecipitation();
    public string WindSpeedAndDirection => GetWindSpeedAndDirection();
    public int WindDirectionArrowRotation => GetWinDirectionArrowRotation();

    public HourlyWeatherForecastItemViewModel(HourlyConditions hourlyConditions)
    {
        this.hourlyConditions = hourlyConditions;
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
        return $"{hourlyConditions.TempC}°C";
    }

    private string GetWeatherDescription()
    {
        return hourlyConditions.WeatherDesc[0].Value;
    }

    private string GetFeelsLikeTemperature()
    {
        return $"Feels like: {hourlyConditions.FeelsLikeC}°C";
    }

    private string GetHumidity()
    {
        return $"Humidity: {hourlyConditions.Humidity}%";
    }

    private string GetPrecipitation()
    {
        return $"Precipitation: {hourlyConditions.PrecipMM} mm";
    }

    private string GetWindSpeedAndDirection()
    {
        return $"{hourlyConditions.WindSpeedKmph}-{hourlyConditions.WindGustKmph} km/h {hourlyConditions.WindDir16Point}";
    }

    private int GetWinDirectionArrowRotation()
    {
        // Add 180 as arrow image points up by default. Arrow would otherwise point in the direction
        // wind comes from instead of where it's going
        return hourlyConditions.WindDirDegree + 180;
    }
}