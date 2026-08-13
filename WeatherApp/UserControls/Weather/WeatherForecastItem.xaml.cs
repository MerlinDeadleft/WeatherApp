using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WeatherApp.UserControls.Weather;

public partial class WeatherForecastItem : UserControl
{
    public WeatherForecastItem()
    {
        InitializeComponent();
    }

    private void HorizontalScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if(e.Handled) return;

        if(sender is ScrollViewer scrollViewer)
        {
            e.Handled = true;

            //Allow horizontal scrolling with shift + mouse scroll
            if(Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - e.Delta);
                return;
            }

            //horizontal scroll view should not block scrolling on vertical scroll view parent, send scroll event up to parent elements
            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = MouseWheelEvent,
                Source = sender
            };
            
            var parent = VisualTreeHelper.GetParent(scrollViewer) as UIElement;
            parent?.RaiseEvent(eventArg);
        }
    }
}