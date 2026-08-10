using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WeatherApp.ViewModels;

namespace WeatherApp.Windows;

public partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<AboutWindowViewModel>();
    }
}