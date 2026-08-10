using System.Windows;
using System.Windows.Input;
using WeatherApp.Helpers;
using WeatherApp.Services;

namespace WeatherApp.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private IDialogService dialogService;

    public ICommand ExitCommand { get; }
    public ICommand OpenAboutWindowCommand { get; }

    public MainWindowViewModel(IDialogService dialogService)
    {
        this.dialogService = dialogService;
        ExitCommand = new RelayAction(ExecuteExitCommand);
        OpenAboutWindowCommand = new RelayAction(ExecuteOpenAboutWindowCommand);
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

    private void ExecuteOpenAboutWindowCommand(object? parameter)
    {
        dialogService.ShowAboutWindow();
    }
}