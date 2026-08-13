using System.Windows;
using System.Windows.Input;
using WeatherApp.Core;
using WeatherApp.Models;
using WeatherApp.Services;

namespace WeatherApp.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private IDialogService dialogService;
    private ISettingsService settingsService;
    
    private bool useMetricUnits;

    public ICommand ExitCommand { get; }
    public ICommand OpenAboutWindowCommand { get; }

    public bool UseMetricUnits
    {
        get => useMetricUnits;
        set
        {
            if(useMetricUnits == value) return;
            useMetricUnits = value;
            settingsService.UpdateSettings(settingsService.GetSettings() with{UseMetric = useMetricUnits});
            DispatchPropertyChanged();
        }
    }

    public MainWindowViewModel(IDialogService dialogService, ISettingsService settingsService)
    {
        this.dialogService = dialogService;
        this.settingsService = settingsService;
        useMetricUnits = settingsService.GetSettings().UseMetric;
        
        ExitCommand = new RelayAction(ExecuteExitCommand);
        OpenAboutWindowCommand = new RelayAction(ExecuteOpenAboutWindowCommand);
    }

    private void ExecuteExitCommand(object? parameter)
    {
        if(Application.Current.MainWindow == null)
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