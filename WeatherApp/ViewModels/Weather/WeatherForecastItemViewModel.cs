using System.Collections.ObjectModel;
using System.Globalization;
using WeatherApp.Core;
using WeatherApp.Models;

namespace WeatherApp.ViewModels.Weather;

public class WeatherForecastItemViewModel : ViewModelBase
{
    private ForecastData forecastData;

    public string ForecastDate => GetForecastDate();
    public string TemperatureRange => GetAverageTemperature();
    
    public ObservableCollection<HourlyWeatherForecastItemViewModel> HourlyWeatherForecasts { get; private set; }

    public WeatherForecastItemViewModel(ForecastData forecast)
    {
        forecastData = forecast;
        var hourlyForecastsViewModels = forecastData.Hourly.Select(x => new HourlyWeatherForecastItemViewModel(x));
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
        var minTemperature = forecastData.Hourly.Min(x => x.TempC);
        var maxTemperature = forecastData.Hourly.Max(x => x.TempC);

        return $"{minTemperature}-{maxTemperature}°C";
    }
}