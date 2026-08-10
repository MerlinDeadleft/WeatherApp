using System.Diagnostics;
using System.Reflection;
using System.Windows.Input;
using WeatherApp.Helpers;

namespace WeatherApp.ViewModels;

public class AboutWindowViewModel
{
    public string AppVersion { get; set; }

    public ICommand OpenGitHubProfileCommand { get; }
    public ICommand OpenGitHubRepoCommand { get; }
    public ICommand OpenWttrWebsiteCommand { get; }
    public ICommand OpenWttrGitHubCommand { get; }

    public AboutWindowViewModel()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        AppVersion = $"Version {version.Major}.{version.Minor}.{version.Build}";

        OpenGitHubProfileCommand = new RelayAction(ExecuteOpenGitHubProfileCommand);
        OpenGitHubRepoCommand = new RelayAction(ExecuteOpenGitHubRepoCommand);
        OpenWttrWebsiteCommand = new RelayAction(ExecuteOpenWttrWebsiteCommand);
        OpenWttrGitHubCommand = new RelayAction(ExecuteOpenWttrGitHubCommand);
    }

    private void ExecuteOpenGitHubProfileCommand(object? parameter)
    {
        Process.Start(new ProcessStartInfo
        {
            UseShellExecute = true,
            FileName = "https://github.com/MerlinDeadleft/"
        });
    }

    private void ExecuteOpenGitHubRepoCommand(object? parameter)
    {
        Process.Start(new ProcessStartInfo
        {
            UseShellExecute = true,
            FileName = "https://github.com/MerlinDeadleft/WeatherApp"
        });
    }

    private void ExecuteOpenWttrWebsiteCommand(object? parameter)
    {
        Process.Start(new ProcessStartInfo
        {
            UseShellExecute = true,
            FileName = "https://wttr.in/"
        });
    }

    private void ExecuteOpenWttrGitHubCommand(object? parameter)
    {
        Process.Start(new ProcessStartInfo
        {
            UseShellExecute = true,
            FileName = "https://github.com/chubin/wttr.in"
        });
    }
}