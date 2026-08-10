using System.Net.Http;
using System.Net.Http.Json;
using WeatherApp.Models;

namespace WeatherApp.Services;

public interface IWeatherService
{
    Task<WeatherModel> GetWeather(string location = "");
}

public class WeatherService : IWeatherService
{
    private HttpClient client;

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

    public async Task<WeatherModel> GetWeather(string location = "")
    {
        try
        {
            var response = await client.GetFromJsonAsync<WeatherData>($"{location ?? ""}?format=j1&lang=en");
            return new WeatherModel
            {
                CityName = string.IsNullOrWhiteSpace(location) ? "IP Based Location" : location,
                Data = response
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        return new WeatherModel
        {
            CityName = "Could not fetch weather data",
        };
    }
}