using WeatherApp.Windows;

namespace WeatherApp.Services;

public interface IDialogService
{
    public void ShowAboutWindow();
}

public class DialogService : IDialogService
{
    public void ShowAboutWindow()
    {
        var about = new AboutWindow();
        about.ShowDialog();
    }
}