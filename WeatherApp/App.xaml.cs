using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WeatherApp.Services;
using WeatherApp.ViewModels;
using WeatherApp.ViewModels.Weather;

namespace WeatherApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static ServiceProvider Services { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<IDialogService, DialogService>();
        serviceCollection.AddSingleton<ISettingsService, SettingsService>();
        serviceCollection.AddSingleton<IWeatherService, WeatherService>();
        
        serviceCollection.AddTransient<MainWindowViewModel>();
        serviceCollection.AddTransient<AboutWindowViewModel>();
        serviceCollection.AddTransient<SidebarViewModel>();
        serviceCollection.AddTransient<WeatherDashboardViewModel>();
        serviceCollection.AddTransient<CurrentWeatherViewModel>();
        serviceCollection.AddTransient<WeatherForecastViewModel>();
        
        Services = serviceCollection.BuildServiceProvider();
    }
}