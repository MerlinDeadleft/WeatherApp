using System.IO;
using System.Text.Json;
using System.Windows;
using WeatherApp.Models;

namespace WeatherApp.Services;

public interface ISettingsService
{
    public enum Setting
    {
        Locations,
        UseMetric
    }
    
    public event Action OnSettingsUpdated;

    public SettingsModel LoadSettings();
    public void UpdateSetting(Setting setting, object value);
}

public class SettingsService : ISettingsService
{
    private static readonly string SettingsFileDirectory = AppContext.BaseDirectory;
    private static readonly string SettingsFileName = "settings.json";

    public event Action OnSettingsUpdated;
    
    private SettingsModel? settingsModel = null;

    public SettingsModel LoadSettings()
    {
        if(settingsModel != null)
        {
            return settingsModel.Value;
        }

        if(File.Exists(Path.Combine(SettingsFileDirectory, SettingsFileName)))
        {
            var json = File.ReadAllText(Path.Combine(SettingsFileDirectory, SettingsFileName));
            settingsModel = JsonSerializer.Deserialize<SettingsModel>(json);
            return settingsModel.Value;
        }

        settingsModel = new SettingsModel()
        {
            Locations = new List<string> { SettingsModel.IpBasedLocationName },
            UseMetric = true
        };

        SaveSettings(settingsModel.Value);
        return settingsModel.Value;
    }

    private void SaveSettings(SettingsModel settings)
    {
        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(Path.Combine(SettingsFileDirectory, SettingsFileName), json);
        OnSettingsUpdated?.Invoke();
    }

    public void UpdateSetting(ISettingsService.Setting setting, object value)
    {
        var settings = settingsModel.Value;
        switch(setting)
        {
            case ISettingsService.Setting.Locations:
                if(value is not List<string> list)
                {
                    MessageBox.Show("Can not update saved locations. Provided value is not of type List<string>.",
                        "Error: SettingsService.UpdateSetting", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                settings.Locations = list;
                break;
            case ISettingsService.Setting.UseMetric:
                if(value is not bool b)
                {
                    MessageBox.Show("Can not update use metric setting. Provided value is not of type bool.",
                        "Error: SettingsService.UpdateSetting", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                settings.UseMetric = b;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(setting), setting, null);
        }

        settingsModel = settings;
        SaveSettings(settingsModel.Value);
    }
}