using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RoutePlaner_Rafael_elias.Services
{
    public class RouteService
    {
        private static readonly string ApiKey = "5b3ce3597851110001cf6248e3c0136fc77342a6b464e761bebb9a25"; 
        private static readonly string BaseUrl = "https://api.openrouteservice.org";

        public async Task<RouteData> GetDirectionsAsync(double startLat, double startLng, double endLat, double endLng)
        {
            using (var client = new HttpClient())
            {
                // Set authorization header with the API key
                client.DefaultRequestHeaders.Add("Authorization", ApiKey);

                // Construct the request URL
                var requestUrl = $"{BaseUrl}/v2/directions/driving-car?start={startLng},{startLat}&end={endLng},{endLat}";

                // Send the GET request to the API
                var response = await client.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    // Read and parse the JSON response
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var routeData = ParseRouteData(jsonString);
                    return routeData;
                }
                else
                {
                    throw new Exception("Error fetching route data: " + response.ReasonPhrase);
                }
            }
        }

        private RouteData ParseRouteData(string jsonString)
        {
            try
            {
                // Parse the JSON to extract relevant data
                var json = JObject.Parse(jsonString);
                var routes = json["routes"]?[0];
                if (routes != null)
                {
                    var geometry = routes["geometry"]?.ToString();
                    var distance = routes["summary"]?["distance"]?.ToObject<double>() ?? 0.0;
                    var duration = routes["summary"]?["duration"]?.ToObject<double>() ?? 0.0;

                    return new RouteData
                    {
                        EncodedPolyline = geometry,
                        Distance = distance,
                        Duration = duration
                    };
                }

                throw new Exception("Invalid route data");
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
