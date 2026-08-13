using WeatherApp.Models;
using WeatherApp.Services;

namespace WeatherApp.ViewModels.Weather.Factories;

public interface IWeatherForecastItemViewModelFactory
{
    WeatherForecastItemViewModel Create(ForecastData forecastData);
}

public class WeatherForecastItemViewModelFactory : IWeatherForecastItemViewModelFactory
{
    private readonly ISettingsService settingsService;
    private readonly IHourlyWeatherForecastItemViewModelFactory hourlyWeatherForecastItemViewModelFactory;

    public WeatherForecastItemViewModelFactory(ISettingsService settingsService, IHourlyWeatherForecastItemViewModelFactory hourlyWeatherForecastItemViewModelFactory)
    {
        this.settingsService = settingsService;
        this.hourlyWeatherForecastItemViewModelFactory = hourlyWeatherForecastItemViewModelFactory;
    }

    public WeatherForecastItemViewModel Create(ForecastData forecastData)
    {
        return new WeatherForecastItemViewModel(forecastData, settingsService, hourlyWeatherForecastItemViewModelFactory);
    }
}