namespace WeatherApp.Models;

public record struct SettingsModel
{
    public const string IpBasedLocationName = "IP Based Location";
    public IReadOnlyList<string> Locations { get; set; }
    public bool UseMetric { get; set; }
}