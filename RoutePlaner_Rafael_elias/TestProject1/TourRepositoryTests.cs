using RoutePlaner_Rafael_elias.Models;
using RoutePlaner_Rafael_elias.Repository;
using System.Collections.ObjectModel;
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
/*
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
*/


}