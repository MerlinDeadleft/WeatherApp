using System.IO;
using System.Text.Json;
using WeatherApp.Models;

namespace WeatherApp.Services;

public interface ISettingsService
{
    event Action OnSettingsUpdated;

    SettingsModel GetSettings();
    void UpdateSettings(SettingsModel settings);
}

public class SettingsService : ISettingsService
{
    private static readonly string SettingsFileDirectory = AppContext.BaseDirectory;
    private static readonly string SettingsFileName = "settings.json";

    public event Action? OnSettingsUpdated;

    private SettingsModel? currentSettings;
    private SettingsModel CurrentSettings
    {
        get
        {
            currentSettings ??= LoadSettings();
            return currentSettings.Value;
        }
        set => currentSettings = value;
    }

    public SettingsModel GetSettings() => CurrentSettings;

    public void UpdateSettings(SettingsModel settings)
    {
        CurrentSettings = settings;
        SaveSettings(CurrentSettings);
    }

    private SettingsModel LoadSettings()
    {
        SettingsModel? settings = null;
        if(File.Exists(Path.Combine(SettingsFileDirectory, SettingsFileName)))
        {
            try
            {
                var json = File.ReadAllText(Path.Combine(SettingsFileDirectory, SettingsFileName));
                settings = JsonSerializer.Deserialize<SettingsModel>(json);
                return settings.Value;
            }
            catch(Exception e)
            {
                Console.WriteLine($"Error loading settings file: {e.Message}\n\nContinuing with fresh save file.");
            }
        }

        settings = new SettingsModel()
        {
            Locations = new List<string> { SettingsModel.IpBasedLocationName },
            UseMetric = true
        };

        SaveSettings(settings.Value, true);
        return settings.Value;
    }

    private void SaveSettings(SettingsModel settings, bool silent = false)
    {
        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(Path.Combine(SettingsFileDirectory, SettingsFileName), json);
        
        if(silent) return;
        OnSettingsUpdated?.Invoke();
    }
}