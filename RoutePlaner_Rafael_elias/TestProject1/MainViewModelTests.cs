using Xunit;
using RoutePlaner_Rafael_elias.ViewModels;
using RoutePlaner_Rafael_elias.Models;

public class MainViewModelTests
{
    [Fact]
    public void SelectedTour_Setter_ShouldCallNotifyCanExecuteChanged()
    {
        // Arrange
        var viewModel = new MainViewModel();
        bool canExecuteChangedTriggered = false;
        viewModel.GenerateReportCommand.CanExecuteChanged += (sender, args) => canExecuteChangedTriggered = true;

        // Act
        viewModel.SelectedTour = new Tour { Name = "Test Tour" };

        // Assert
        Assert.True(canExecuteChangedTriggered);
    }

    [Fact]
    public void SelectedLog_Setter_ShouldCallNotifyCanExecuteChanged()
    {
        // Arrange
        var viewModel = new MainViewModel();
        bool canExecuteChangedTriggered = false;
        viewModel.UpdateLogCommand.CanExecuteChanged += (sender, args) => canExecuteChangedTriggered = true;

        // Act
        viewModel.SelectedLog = new Log { Id = 1, TourId = 1 };

        // Assert
        Assert.True(canExecuteChangedTriggered);
    }
}