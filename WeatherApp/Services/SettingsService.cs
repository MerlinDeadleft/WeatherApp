using System.IO;
using System.Text.Json;
using WeatherApp.Models;

namespace WeatherApp.Services;

public interface ISettingsService
{
    public SettingsModel LoadSettings();
    public void SaveSettings(SettingsModel settings);
}

public class SettingsService : ISettingsService
{
    private static readonly string SettingsFileDirectory = AppContext.BaseDirectory;
    private static readonly string SettingsFileName = "settings.json";

    private SettingsModel? settingsModel = null;

    public SettingsModel LoadSettings()
    {
        if (settingsModel != null)
        {
            return settingsModel.Value;
        }

        if (File.Exists(Path.Combine(SettingsFileDirectory, SettingsFileName)))
        {
            var json = File.ReadAllText(Path.Combine(SettingsFileDirectory, SettingsFileName));
            return JsonSerializer.Deserialize<SettingsModel>(json);
        }

        settingsModel = new SettingsModel()
        {
            Locations = new List<string> { SettingsModel.IpBasedLocationName },
        };

        SaveSettings(settingsModel.Value);
        return settingsModel.Value;
    }

    public void SaveSettings(SettingsModel settings)
    {
        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(Path.Combine(SettingsFileDirectory, SettingsFileName), json);
    }
}