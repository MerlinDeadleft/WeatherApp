using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using WeatherApp.ViewModels.Weather;

namespace WeatherApp.UserControls.Weather;

public partial class WeatherDashboard : UserControl
{
    public WeatherDashboard()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<WeatherDashboardViewModel>();
    }
}