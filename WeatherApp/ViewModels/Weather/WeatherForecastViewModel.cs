using System.Collections.ObjectModel;
using WeatherApp.Core;
using WeatherApp.Models;

namespace WeatherApp.ViewModels.Weather;

public class WeatherForecastViewModel : ViewModelBase
{
    private WeatherModel? weatherModel;

    public ObservableCollection<WeatherForecastItemViewModel> WeatherForecasts { get; private set; }
    
    public void UpdateWeatherModel(WeatherModel? weatherModel)
    {
        this.weatherModel = weatherModel;
        if(weatherModel == null)
        {
            WeatherForecasts?.Clear();
            return;
        }

        var itemViewModels = weatherModel.Data.Value.Weather.Select(CreateWeatherForecastItemViewModels);
        WeatherForecasts = new ObservableCollection<WeatherForecastItemViewModel>(itemViewModels);
        DispatchPropertyChanged(null);
    }

    private WeatherForecastItemViewModel CreateWeatherForecastItemViewModels(ForecastData forecastData)
    {
        return new WeatherForecastItemViewModel(forecastData);
    }
}