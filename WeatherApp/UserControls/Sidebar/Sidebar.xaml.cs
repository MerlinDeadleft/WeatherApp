using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
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

    private void AddLocationTextBox_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if(sender is UIElement element && (bool)e.NewValue)
        {
            element.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
            {
                element.Focus();
                System.Windows.Input.Keyboard.Focus(element);
            }));
        }
    }
}