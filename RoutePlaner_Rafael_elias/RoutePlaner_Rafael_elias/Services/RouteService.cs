using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RoutePlaner_Rafael_elias.Services
{
    public class RouteService
    {
        private static readonly string ApiKey = "5b3ce3597851110001cf6248e3c0136fc77342a6b464e761bebb9a25"; // Replace with your actual API key
        private static readonly string BaseUrl = "https://api.openrouteservice.org";

        public async Task<RouteData> GetDirectionsAsync(double startLat, double startLng, double endLat, double endLng, string routeType)
        {
            using (var client = new HttpClient())
            {
                // Format coordinates to six decimal places using dots
                string formattedStartLng = FormatCoordinate(startLng);
                string formattedStartLat = FormatCoordinate(startLat);
                string formattedEndLng = FormatCoordinate(endLng);
                string formattedEndLat = FormatCoordinate(endLat);

                // Determine the correct route profile based on routeType
                string routeProfile = routeType switch
                {
                    "bike" => "cycling-regular",
                    "walking" => "foot-walking",
                    "driving-car" => "driving-car",
                    _ => "driving-car" // Default to driving-car if no match
                };

                // Construct the request URL with the API key and formatted coordinates
                var requestUrl = $"{BaseUrl}/v2/directions/{routeProfile}?api_key={ApiKey}&start={formattedStartLng},{formattedStartLat}&end={formattedEndLng},{formattedEndLat}";

                // Log the request URL
                Console.WriteLine($"Request URL: {requestUrl}");

                // Send the GET request to the API
                var response = await client.GetAsync(requestUrl);

                // Log the response status code and content
                Console.WriteLine($"Response Status Code: {response.StatusCode}");
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response JSON: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var routeData = ParseRouteData(responseContent);
                    return routeData;
                }
                else
                {
                    throw new Exception($"Error fetching route data: {response.ReasonPhrase}. Response: {responseContent}");
                }
            }
        }

        public string FormatCoordinate(double coordinate)
        {
            // Format the coordinate to six decimal places with dots
            return coordinate.ToString("F6", CultureInfo.InvariantCulture);
        }

        public RouteData ParseRouteData(string jsonString)
        {
            try
            {
                // Parse the JSON to extract relevant data
                var json = JObject.Parse(jsonString);
                var routes = json["features"]?[0];
                if (routes != null)
                {
                    var geometry = routes["geometry"]?["coordinates"]?.ToString();
                    var properties = routes["properties"];
                    var segments = properties?["segments"]?[0];
                    var distance = segments?["distance"]?.ToObject<double>() ?? 0.0;
                    var duration = segments?["duration"]?.ToObject<double>() ?? 0.0;

                    return new RouteData
                    {
                        EncodedPolyline = geometry,
                        Distance = distance,
                        Duration = duration
                    };
                }

                throw new Exception("Invalid route data: No route features found");
            }
            catch (JsonException ex)
            {
                throw new Exception("Error parsing route data: " + ex.Message);
            }
        }
    }

    public class RouteData
    {
        public string EncodedPolyline { get; set; }
        public double Distance { get; set; }
        public double Duration { get; set; }
    }
}
