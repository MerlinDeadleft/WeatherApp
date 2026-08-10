using System.Text.Json.Serialization;

namespace WeatherApp.Models;

public class WeatherModel
{
    public string CityName { get; set; } = "";
    public WeatherData Data { get; set; }
}

public struct WeatherData
{
    [JsonPropertyName("current_condition")]
    public List<Conditions> CurrentCondition { get; set; }

    [JsonPropertyName("nearest_area")] public List<LocationData> LocationData { get; set; }
    public List<ForecastData> Weather { get; set; }
}

public struct DataString
{
    public string Value { get; set; }
}

public abstract class ConditionsBase
{
    public int FeelsLikeC { get; set; }
    public int FeelsLikeF { get; set; }
    public int Humidity { get; set; }
    public float PrecipInches { get; set; }
    public float PrecipMM { get; set; }
    public List<DataString> WeatherDesc { get; set; } = new List<DataString>();
    public List<DataString> WeatherIconUrl { get; set; } = new List<DataString>();
    public string WindDir16Point { get; set; } = "";
    public int WindDirDegree { get; set; }
    public int WindSpeedKmph { get; set; }
    public int WindSpeedMiles { get; set; }
}

public class Conditions : ConditionsBase
{
    [JsonPropertyName("observation_time")] public string ObservationTime { get; set; } = "";

    [JsonPropertyName("temp_C")] public int TempC { get; set; }
    [JsonPropertyName("temp_F")] public int TempF { get; set; }
}

public class HourlyConditions : ConditionsBase
{
    public string Time { get; set; } = "";
    public int TempC { get; set; }
    public int TempF { get; set; }
}

public struct LocationData
{
    public List<DataString> AreaName { get; set; }
    public List<DataString> Region { get; set; }
    public List<DataString> Country { get; set; }
    public float Latitude { get; set; }
    public float Longitude { get; set; }
}

public struct ForecastData
{
    public string Date { get; set; }
    public int AvgTempC { get; set; }
    public int AvgTempF { get; set; }
    public List<HourlyConditions> Hourly { get; set; }
}