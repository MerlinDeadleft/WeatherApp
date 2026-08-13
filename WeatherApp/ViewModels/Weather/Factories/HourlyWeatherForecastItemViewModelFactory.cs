using WeatherApp.Models;
using WeatherApp.Services;

namespace WeatherApp.ViewModels.Weather.Factories;

public interface IHourlyWeatherForecastItemViewModelFactory
{
    HourlyWeatherForecastItemViewModel Create(HourlyConditions conditions);
}

public class HourlyWeatherForecastItemViewModelFactory : IHourlyWeatherForecastItemViewModelFactory
{
    private readonly ISettingsService settingsService;

    public HourlyWeatherForecastItemViewModelFactory(ISettingsService settingsService)
    {
        this.settingsService = settingsService;
    }

    public HourlyWeatherForecastItemViewModel Create(HourlyConditions conditions)
    {
        return new HourlyWeatherForecastItemViewModel(conditions, settingsService);
    }
}