namespace RoutePlaner_Rafael_elias.Services;

// Pfad: Services/DistanceService.cs
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class DistanceService
{
    private readonly string apiKey = "YOUR_API_KEY";
    private readonly HttpClient httpClient;

    public DistanceService()
    {
        httpClient = new HttpClient();
    }

    public async Task<string> GetDistanceAsync(double[] start, double[] end)
    {
        var requestBody = new
        {
            locations = new double[][] { start, end },
            metrics = new string[] { "distance", "duration" }
        };
        
        string url = $"https://api.openrouteservice.org/v2/matrix/driving-car?api_key={apiKey}";

        StringContent content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
        HttpResponseMessage response = await httpClient.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            string jsonResult = await response.Content.ReadAsStringAsync();
            return jsonResult;
        }
        else
        {
            throw new Exception("Failed to retrieve distance data.");
        }
    }
}
