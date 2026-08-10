using System.Windows;
using System.Windows.Input;
using WeatherApp.Helpers;

namespace WeatherApp.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ICommand ExitCommand { get; }

    public MainWindowViewModel()
    {
        ExitCommand = new RelayAction(ExecuteExitCommand);
    }

    private void ExecuteExitCommand(object? parameter)
    {
        if (Application.Current.MainWindow == null)
        {
            MessageBox.Show("Can not exit application as it has no main window!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        Application.Current.MainWindow?.Close();
    }
}