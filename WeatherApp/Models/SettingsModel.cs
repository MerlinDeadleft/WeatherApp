namespace WeatherApp.Models;

public struct SettingsModel
{
    public const string IpBasedLocationName = "IP Based Location";
    public List<string> Locations { get; set; }
    public bool UseMetric { get; set; }
}