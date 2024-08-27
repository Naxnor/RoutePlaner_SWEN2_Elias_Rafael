namespace RoutePlaner_Rafael_elias.Services;

// Pfad: Services/RouteService.cs
using System;
using System.Net.Http;
using System.Threading.Tasks;

public class RouteService
{
    private readonly string apiKey = "5b3ce3597851110001cf62488811f18c94944f8890c9e1fb28b1002f";
    private readonly HttpClient httpClient;

    public RouteService()
    {
        httpClient = new HttpClient();
    }

    public async Task<string> GetRouteAsync(string startLocation, string endLocation)
    {
        string url = $"https://api.openrouteservice.org/v2/directions/driving-car?api_key={apiKey}&start={startLocation}&end={endLocation}";
        
        HttpResponseMessage response = await httpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            string jsonResult = await response.Content.ReadAsStringAsync();
            return jsonResult;
        }
        else
        {
            throw new Exception("Failed to retrieve route data.");
        }
    }
}
