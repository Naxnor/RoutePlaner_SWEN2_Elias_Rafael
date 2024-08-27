namespace RoutePlaner_Rafael_elias.Helpers;

// Pfad: Helpers/JsonHelper.cs
using Newtonsoft.Json.Linq;

public class JsonHelper
{
    public double ExtractDistance(string jsonResponse)
    {
        var data = JObject.Parse(jsonResponse);
        double distance = data["routes"][0]["summary"]["distance"].Value<double>();
        return distance; // Distance in meters
    }

    public double ExtractDuration(string jsonResponse)
    {
        var data = JObject.Parse(jsonResponse);
        double duration = data["routes"][0]["summary"]["duration"].Value<double>();
        return duration; // Duration in seconds
    }
}
