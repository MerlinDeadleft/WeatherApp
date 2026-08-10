using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using WeatherApp.ViewModels;

namespace WeatherApp.UserControls.Sidebar;

public partial class Sidebar : UserControl
{
    public Sidebar()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<SidebarViewModel>();
    }
}