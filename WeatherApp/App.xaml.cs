using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WeatherApp.Core;
using WeatherApp.Services;
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

        serviceCollection.AddSingleton<IDialogService, DialogService>();
        serviceCollection.AddSingleton<ISettingsService, SettingsService>();
        serviceCollection.AddSingleton<IMessenger, Messenger>();
        
        serviceCollection.AddTransient<MainWindowViewModel>();
        serviceCollection.AddTransient<AboutWindowViewModel>();
        serviceCollection.AddTransient<SidebarViewModel>();
        
        Services = serviceCollection.BuildServiceProvider();
    }
}