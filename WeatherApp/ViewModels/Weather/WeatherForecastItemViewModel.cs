using System.Collections.ObjectModel;
using System.Globalization;
using WeatherApp.Core;
using WeatherApp.Models;
using WeatherApp.Services;
using WeatherApp.ViewModels.Weather.Factories;

namespace WeatherApp.ViewModels.Weather;

public class WeatherForecastItemViewModel : ViewModelBase
{
    private ForecastData forecastData;
    private ISettingsService settingsService;
    private SettingsModel settingsModel;

    public string ForecastDate => GetForecastDate();
    public string TemperatureRange => GetAverageTemperature();

    public ObservableCollection<HourlyWeatherForecastItemViewModel> HourlyWeatherForecasts { get; private set; }

    public WeatherForecastItemViewModel(ForecastData forecast, ISettingsService settingsService, IHourlyWeatherForecastItemViewModelFactory hourlyWeatherForecastItemViewModelFactory)
    {
        forecastData = forecast;
        this.settingsService = settingsService;
        this.settingsService.OnSettingsUpdated += HandleSettingsUpdated;
        settingsModel = settingsService.LoadSettings();
        var hourlyForecastsViewModels = forecastData.Hourly.Select(x => hourlyWeatherForecastItemViewModelFactory.Create(x));
        HourlyWeatherForecasts = new ObservableCollection<HourlyWeatherForecastItemViewModel>(hourlyForecastsViewModels);
    }

    private string GetForecastDate()
    {
        if(!DateTime.TryParseExact(forecastData.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            return forecastData.Date;
        }

        return date.ToString("D");
    }

    private string GetAverageTemperature()
    {
        var minTemperature = settingsModel.UseMetric
            ? forecastData.Hourly.Min(x => x.TempC)
            : forecastData.Hourly.Min(x => x.TempF);
        var maxTemperature = settingsModel.UseMetric
            ? forecastData.Hourly.Max(x => x.TempC)
            : forecastData.Hourly.Max(x => x.TempF);

        return settingsModel.UseMetric
            ? $"{minTemperature}-{maxTemperature}°C"
            : $"{minTemperature}-{maxTemperature}°F";
    }

    private void HandleSettingsUpdated()
    {
        settingsModel = settingsService.LoadSettings();
        DispatchPropertyChanged(null);
    }
}