namespace WeatherApp.Messages;

public class LocationSelectMessage
{
    public string LocationName { get; }

    public LocationSelectMessage(string locationName)
    {
        LocationName = locationName;
    }
}