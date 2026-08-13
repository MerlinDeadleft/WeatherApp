using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using WeatherApp.ViewModels.Weather;

namespace WeatherApp.UserControls.Weather;

public partial class WeatherDashboard : UserControl
{
    public WeatherDashboard()
    {
        InitializeComponent();
        var viewModel = App.Services.GetRequiredService<WeatherDashboardViewModel>();
        DataContext = viewModel;

        viewModel.OnWeatherDataUpdated += HandleWeatherDataUpdated;
    }

    public void HandleWeatherDataUpdated()
    {
        Application.Current.Dispatcher.InvokeAsync(() =>
        {
            ForecastScrollViewer.ScrollToHome();
        });
    }
}