using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using WeatherApp.Models;

namespace WeatherApp.Services;

public interface IWeatherService
{
    event Action SelectedLocationChanged;

    string SelectedLocation { get; set; }
    bool IsFetching { get; }

    Task<WeatherModel> GetWeather(string location);
}

public class WeatherService : IWeatherService
{
    private HttpClient client;

    public event Action SelectedLocationChanged;

    public string SelectedLocation
    {
        get;
        set
        {
            if(field == value) return;
            field = value;
            SelectedLocationChanged?.Invoke();
        }
    }

    public bool IsFetching { get; private set; }

    public WeatherService()
    {
        var handler = new SocketsHttpHandler()
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(15),
        };

        client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://wttr.in")
        };
    }

    public async Task<WeatherModel> GetWeather(string location)
    {
        try
        {
            IsFetching = true;
            var response = await client.GetFromJsonAsync<WeatherData>($"{(location == SettingsModel.IpBasedLocationName ? "" : location)}?format=j1&lang=en");
            IsFetching = false;
            return new WeatherModel
            {
                LocationName = string.IsNullOrWhiteSpace(location) ? SettingsModel.IpBasedLocationName : location,
                Data = response
            };
        }
        catch(Exception e)
        {
            MessageBox.Show("There was an error fetching the weather data:\n" + e.Message, "Error: WeatherService", MessageBoxButton.OK, MessageBoxImage.Error);
            Console.WriteLine(e.Message);
            IsFetching = false;
        }

        return new WeatherModel
        {
            LocationName = string.IsNullOrWhiteSpace(location) ? SettingsModel.IpBasedLocationName : location,
            Data = null
        };
    }
}