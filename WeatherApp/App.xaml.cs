using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WeatherApp.ViewModels;

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
        
        serviceCollection.AddTransient<MainWindowViewModel>();
        
        Services = serviceCollection.BuildServiceProvider();
    }
}