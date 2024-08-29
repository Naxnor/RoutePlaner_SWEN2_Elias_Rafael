using System;
using RoutePlaner_Rafael_elias.Models;
using RoutePlaner_Rafael_elias.Repository;
using System.Collections.ObjectModel;
using System.Linq;
using Xunit;

public class TourRepositoryTests
{
    private readonly TourRepository _tourRepository;

    public TourRepositoryTests()
    {
        _tourRepository = new TourRepository();
    }

    [Fact]
    public void GetAllTours_ReturnsAllTours()
    {
        // Act
        ObservableCollection<Tour> tours = _tourRepository.GetAllTours();

        // Assert
        Assert.NotNull(tours);
        Assert.NotEmpty(tours);
    }

    [Fact]
    public void AddTour_ValidTour_AddsTourToDatabase()
    {
        // Arrange
        var tour = new Tour { Name = "Test Tour", From = "Start", To = "End", RouteType = "walking" };

        // Act
        _tourRepository.AddTour(tour);

        // Assert
        // Fetch the tour again or check for its existence
        var fetchedTours = _tourRepository.GetAllTours();
        Assert.Contains(fetchedTours, t => t.Name == "Test Tour" && t.From == "Start" && t.To == "End");
    }
    
    [Fact]
    public void UpdateTour_ValidTour_UpdatesTourInDatabase()
    {
        // Arrange
        var tour = new Tour { Name = "Old Tour", From = "Old Start", To = "Old End", RouteType = "walking" };
        _tourRepository.AddTour(tour);
        tour.Name = "Updated Tour";
        tour.From = "Updated Start";

        // Act
        _tourRepository.UpdateTour(tour);

        // Assert
        var updatedTour = _tourRepository.GetTourById(tour.Id);
        Assert.Equal("Updated Tour", updatedTour.Name);
        Assert.Equal("Updated Start", updatedTour.From);
    }

    [Fact]
    public void DeleteTour_ValidTour_RemovesTourFromDatabase()
    {
        // Arrange
        var tour = new Tour { Name = "Tour to Delete", From = "Start", To = "End", RouteType = "walking" };
        _tourRepository.AddTour(tour);

        // Act
        _tourRepository.DeleteTour(tour);

        // Assert
        var remainingTours = _tourRepository.GetAllTours();
        Assert.DoesNotContain(remainingTours, t => t.Name == "Tour to Delete");
    }

    [Fact]
    public void GetAllToursWithLogs_ReturnsToursWithLogs()
    {
        // Act
        var toursWithLogs = _tourRepository.GetAllToursWithLogs();

        // Assert
        Assert.NotNull(toursWithLogs);
        Assert.All(toursWithLogs, tour => Assert.NotNull(tour.Logs));
    }

    [Fact]
    public void GetTourById_ValidId_ReturnsCorrectTour()
    {
        // Arrange
        var tour = new Tour { Name = "Specific Tour", From = "Start", To = "End", RouteType = "walking" };
        _tourRepository.AddTour(tour);

        // Act
        var fetchedTour = _tourRepository.GetTourById(tour.Id);

        // Assert
        Assert.NotNull(fetchedTour);
        Assert.Equal(tour.Name, fetchedTour.Name);
        Assert.Equal(tour.From, fetchedTour.From);
        Assert.Equal(tour.To, fetchedTour.To);
    }

    [Fact]
    public void SearchTours_ValidQuery_ReturnsMatchingTours()
    {
        // Arrange
        var tour = new Tour { Name = "TourWithLogs", From = "Start", To = "End", RouteType = "walking" };
        _tourRepository.AddTour(tour);
    
        // Act
        var results = _tourRepository.SearchTours("TourWithLogs");

        // Assert
        Assert.NotNull(results);
        Assert.Contains(results, t => t.Name == "TourWithLogs");
    }

    
    [Fact]
    public void SearchTours_NoMatchingResults_ReturnsEmptyCollection()
    {
        // Act
        var results = _tourRepository.SearchTours("NonExistentTourName");

        // Assert
        Assert.NotNull(results);
        Assert.Empty(results);
    }

    [Fact]
    public void GetRouteTypeForTour_ValidTourId_ReturnsRouteType()
    {
        // Arrange
        var tour = new Tour { Name = "TourWithRouteType", From = "Start", To = "End", RouteType = "driving-car" };
        _tourRepository.AddTour(tour);

        // Act
        var routeType = _tourRepository.GetRouteTypeForTour(tour.Id);

        // Assert
        Assert.Equal("driving-car", routeType);
    }

    [Fact]
    public void GetAllToursWithLogs_NoLogs_ReturnsToursWithEmptyLogs()
    {
        // Arrange
        var tour = new Tour { Name = "TourWithoutLogs", From = "Start", To = "End", RouteType = "walking" };
        _tourRepository.AddTour(tour);

        // Act
        var toursWithLogs = _tourRepository.GetAllToursWithLogs();

        // Assert
        Assert.NotNull(toursWithLogs);
        var tourWithoutLogs = toursWithLogs.FirstOrDefault(t => t.Name == "TourWithoutLogs");
        Assert.NotNull(tourWithoutLogs);
        Assert.Empty(tourWithoutLogs.Logs);
    }

    
    [Fact]
    public void UpdateLog_ValidLog_UpdatesLogInDatabase()
    {
        // Arrange
        var tour = new Tour { Name = "TourWithLog", From = "Start", To = "End", RouteType = "walking" };
        _tourRepository.AddTour(tour);

        var log = new Log { TourId = tour.Id, Date = DateTime.Now, Distance = 10, Difficulty = 5, Duration = 2, Steps = 5000, Weather = "Sunny", TotalTime = 2.5m, Comment = "Initial comment", Rating = 4 };
        _tourRepository.AddLog(log);

        log.Comment = "Updated comment";

        // Act
        _tourRepository.UpdateLog(log);

        // Assert
        var logs = _tourRepository.GetLogsForTour(tour).ToList();
        Assert.Contains(logs, l => l.Comment == "Updated comment");
    }

    [Fact]
    public void DeleteLog_ValidLog_RemovesLogFromDatabase()
    {
        // Arrange
        var tour = new Tour { Name = "TourWithLogToDelete", From = "Start", To = "End", RouteType = "walking" };
        _tourRepository.AddTour(tour);

        var log = new Log { TourId = tour.Id, Date = DateTime.Now, Distance = 10, Difficulty = 5, Duration = 2, Steps = 5000, Weather = "Sunny", TotalTime = 2.5m, Comment = "To be deleted", Rating = 4 };
        _tourRepository.AddLog(log);

        // Act
        _tourRepository.DeleteLog(log);

        // Assert
        var logs = _tourRepository.GetLogsForTour(tour).ToList();
        Assert.DoesNotContain(logs, l => l.Comment == "To be deleted");
    }
}