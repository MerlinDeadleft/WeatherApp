using System.Windows.Input;
using WeatherApp.Core;
using WeatherApp.Models;
using WeatherApp.Services;

namespace WeatherApp.ViewModels.Weather;

public class WeatherDashboardViewModel : ViewModelBase
{
    private IWeatherService weatherService;
    private WeatherModel? weatherModel = null;
    
    public CurrentWeatherViewModel CurrentWeatherViewModel { get; }
    
    public ICommand RefreshDataCommand { get; }

    public WeatherDashboardViewModel(IWeatherService weatherService, CurrentWeatherViewModel currentWeatherViewModel)
    {
        CurrentWeatherViewModel = currentWeatherViewModel;
        
        this.weatherService = weatherService;
        this.weatherService.SelectedLocationChanged += HandleSelectedLocationChanged;
        FetchWeather();

        RefreshDataCommand = new RelayAction(ExecuteRefreshDataCommand, CanExecuteRefreshDataCommand);
    }

    private void FetchWeather()
    {
        weatherModel = null;
        PushWeatherModelToViewModels();
        Task.Run(() => weatherService.GetWeather(weatherService.SelectedLocation))
            .ContinueWith(task =>
            {
                weatherModel = task.Result;
                PushWeatherModelToViewModels();
            });
    }

    private void PushWeatherModelToViewModels()
    {
        CurrentWeatherViewModel.UpdateWeatherModel(weatherModel);
    }

    private void HandleSelectedLocationChanged()
    {
        FetchWeather();
    }

    private bool CanExecuteRefreshDataCommand(object? parameter)
    {
        return !weatherService.IsFetching;
    }
    
    private void ExecuteRefreshDataCommand(object? parameter)
    {
        FetchWeather();
    }
}