using System;
using System.Threading.Tasks;
using RoutePlaner_Rafael_elias.Services;
using Xunit;

public class RouteServiceTests
{
    private readonly RouteService _routeService;

    public RouteServiceTests()
    {
        _routeService = new RouteService();
    }

    [Fact]
    public async Task GetDirectionsAsync_ValidInput_ReturnsValidRouteData()
    {
        // Arrange
        double startLat = 49.41461;
        double startLng = 8.681495;
        double endLat = 49.420318;
        double endLng = 8.687872;
        string routeType = "driving-car";

        // Act
        var result = await _routeService.GetDirectionsAsync(startLat, startLng, endLat, endLng, routeType);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.EncodedPolyline);
    }
    
    [Fact]
    public async Task GetDirectionsAsync_InvalidInput_ThrowsException()
    {
        // Arrange
        double invalidLat = -999;
        double invalidLng = -999;
        string routeType = "driving-car";
        var routeService = new RouteService();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => routeService.GetDirectionsAsync(invalidLat, invalidLng, invalidLat, invalidLng, routeType));
    }

    [Fact]
    public void FormatCoordinate_ValidInput_ReturnsFormattedString()
    {
        // Arrange
        double coordinate = 49.4146135;

        // Act
        string result = _routeService.FormatCoordinate(coordinate);

        // Assert
        Assert.Equal("49.414614", result);
    }
    
    [Fact]
    public void ParseRouteData_ValidJson_ReturnsRouteData()
    {
        // Arrange
        string validJson = @"{
        'features': [{
            'geometry': {
                'coordinates': 'encodedPolylineString'
            },
            'properties': {
                'segments': [{
                    'distance': 1200.0,
                    'duration': 300.0
                }]
            }
        }]
        }";
    
        // Act
        var routeData = _routeService.ParseRouteData(validJson);

        // Assert
        Assert.NotNull(routeData);
        Assert.Equal("encodedPolylineString", routeData.EncodedPolyline);
        Assert.Equal(1200.0, routeData.Distance);
        Assert.Equal(300.0, routeData.Duration);
    }

    [Fact]
    public void ParseRouteData_InvalidJson_ThrowsJsonException()
    {
        // Arrange
        string invalidJson = @"{ 'invalid': 'json'";

        // Act & Assert
        Assert.Throws<Exception>(() => _routeService.ParseRouteData(invalidJson));
    }

    
}