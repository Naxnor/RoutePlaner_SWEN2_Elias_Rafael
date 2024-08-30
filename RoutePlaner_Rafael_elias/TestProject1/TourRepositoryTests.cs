using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RoutePlaner_Rafael_elias.Database;
using RoutePlaner_Rafael_elias.Models;
using RoutePlaner_Rafael_elias.Repository;
using Xunit;

public class TourRepositoryTests
{
    private readonly TourRepository _tourRepository;
    private readonly ApplicationDbContext _context;

    public TourRepositoryTests()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "RoutePlanerTestDb")
            .Options;

        _context = new ApplicationDbContext(options);
        _tourRepository = new TourRepository(_context);
    }

    private Tour CreateValidTour(string name = "Default Tour")
    {
        return new Tour
        {
            Name = name,
            From = "Start",
            To = "End",
            RouteType = "walking",
            Description = "A valid test tour",
            Distance = 10.0,
            EstimatedTime = new TimeSpan(2, 0, 0)
        };
    }

    [Fact]
    public void GetAllTours_ReturnsAllTours()
    {
        // Cleanup existing data to ensure test isolation
        _context.Tours.RemoveRange(_context.Tours);
        _context.SaveChanges();

        // Arrange
        var tour1 = CreateValidTour("Tour1");
        var tour2 = CreateValidTour("Tour2");
        _context.Tours.AddRange(tour1, tour2);
        _context.SaveChanges();

        // Act
        var tours = _tourRepository.GetAllTours();

        // Assert
        Assert.NotNull(tours);
        Assert.Equal(2, tours.Count); // Erwartete Anzahl an Touren: 2
    }


    [Fact]
    public void AddTour_ValidTour_AddsTourToDatabase()
    {
        // Arrange
        var tour = CreateValidTour("Test Tour");

        // Act
        _tourRepository.AddTour(tour);

        // Assert
        var fetchedTours = _tourRepository.GetAllTours();
        Assert.Contains(fetchedTours, t => t.Name == "Test Tour" && t.From == "Start" && t.To == "End");
    }

    [Fact]
    public void UpdateTour_ValidTour_UpdatesTourInDatabase()
    {
        // Arrange
        var tour = CreateValidTour("Old Tour");
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
        var tour = CreateValidTour("Tour to Delete");
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
        // Arrange
        var tour = CreateValidTour("TourWithLogs");
        var log = new Log
        {
            Date = DateTime.Now,
            Distance = 5,
            Difficulty = 3,
            Duration = 60,
            Tour = tour,
            Comment = "Great hike!",
            Weather = "Sunny"
        };
        tour.Logs = new[] { log };
        _context.Tours.Add(tour);
        _context.SaveChanges();

        // Act
        var toursWithLogs = _tourRepository.GetAllToursWithLogs();

        // Assert
        Assert.NotNull(toursWithLogs);
        Assert.All(toursWithLogs, t => Assert.NotNull(t.Logs));
    }


    [Fact]
    public void GetTourById_ValidId_ReturnsCorrectTour()
    {
        // Arrange
        var tour = CreateValidTour("Specific Tour");
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
        var tour = CreateValidTour("TourWithLogs");
        _tourRepository.AddTour(tour);

        // Act
        var results = _tourRepository.SearchTours("TourWithLogs");

        // Assert
        Assert.NotNull(results);
        Assert.Contains(results, t => t.Name.Contains("TourWithLogs"));
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
        var tour = CreateValidTour("TourWithRouteType");
        tour.RouteType = "driving-car";
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
        var tour = CreateValidTour("TourWithoutLogs");
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
        var tour = CreateValidTour("TourWithLog");
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
        var tour = CreateValidTour("TourWithLogToDelete");
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
