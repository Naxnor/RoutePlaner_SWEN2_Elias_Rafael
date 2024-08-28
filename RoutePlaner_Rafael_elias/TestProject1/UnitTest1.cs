
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Moq;
using RoutePlaner_Rafael_elias.ViewModels;  
using RoutePlaner_Rafael_elias.Models;    


public class UnitTest1
{
    public interface ITourRepository
    {
        void AddLog(Log log);
        void DeleteLog(Log log);
        Log GetLogById(int id);
        IEnumerable<Log> GetAllLogs();
        
    }
    
  
    [Fact]
    public void UpdateLogCommand_CanExecute_ReturnsTrueWhenLogIsSelected()
    {
        // Arrange
        var mockRepository = new Mock<ITourRepository>();
        var viewModel = new MainViewModel();
        var log = new Log { Id = 1, TourId = 1 }; // Ensure log details are sufficient
        viewModel.LogSelection = log; // Correct property set

        // Act
        bool canExecute = viewModel.UpdateLogCommand.CanExecute(null);

        // Assert
        Assert.True(canExecute); // Verifies that the command can execute
    }


    [Fact]
    public void UpdateLogCommand_CanExecute_ReturnsFalseWhenNoLogIsSelected()
    {
        // Arrange
        var mockRepository = new Mock<ITourRepository>();
        var viewModel = new MainViewModel();

        // Act
        bool canExecute = viewModel.UpdateLogCommand.CanExecute(null);

        // Assert
        Assert.False(canExecute);
    }


   

    [Fact]
    public void Test_DeleteLogCommand_CanExecute()
    {
        // Arrange
        var viewModel = new MainViewModel();
        viewModel.SelectedLog = new Log { Id = 1 };

        // Act
        bool canExecute = viewModel.DeleteLogCommand.CanExecute(null);

        // Assert
        Assert.True(canExecute);
    }

    [Fact]
    public void Test_DeleteLogCommand_CannotExecute_WhenLogIsNull()
    {
        // Arrange
        var viewModel = new MainViewModel();

        // Act
        bool canExecute = viewModel.DeleteLogCommand.CanExecute(null);

        // Assert
        Assert.False(canExecute);
    }
}
