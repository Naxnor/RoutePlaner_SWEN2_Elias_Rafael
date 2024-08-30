using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RoutePlaner_Rafael_elias.Database;
using RoutePlaner_Rafael_elias.Models;
using RoutePlaner_Rafael_elias.Repository;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace RoutePlaner_Rafael_elias.Tests
{
    public class Extra
    {/*
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Verwende eine neue In-Memory-Datenbank für jeden Testlauf
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public void GetAllTours_ReturnsAllTours()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new TourRepository(context);

            var tours = new List<Tour>
            {
                new Tour { Name = "Tour 1", Description = "Description 1" },
                new Tour { Name = "Tour 2", Description = "Description 2" }
            };
            context.Tours.AddRange(tours);
            context.SaveChanges();

            // Act
            var result = repository.GetAllTours();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, t => t.Name == "Tour 1");
            Assert.Contains(result, t => t.Name == "Tour 2");
        }

        [Fact]
        public void AddTour_AddsNewTour()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new TourRepository(context);

            var newTour = new Tour { Name = "New Tour", Description = "New Description" };

            // Act
            repository.AddTour(newTour);
            var result = context.Tours.FirstOrDefault(t => t.Name == "New Tour");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Tour", result.Name);
            Assert.Equal("New Description", result.Description);
        }

        [Fact]
        public void UpdateTour_UpdatesExistingTour()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new TourRepository(context);

            var tour = new Tour { Name = "Tour to Update", Description = "Old Description" };
            context.Tours.Add(tour);
            context.SaveChanges();

            // Act
            tour.Description = "Updated Description";
            repository.UpdateTour(tour);
            var result = context.Tours.FirstOrDefault(t => t.Name == "Tour to Update");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Description", result.Description);
        }

        [Fact]
        public void DeleteTour_RemovesTour()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new TourRepository(context);

            var tour = new Tour { Name = "Tour to Delete" };
            context.Tours.Add(tour);
            context.SaveChanges();

            // Act
            repository.DeleteTour(tour);
            var result = context.Tours.FirstOrDefault(t => t.Name == "Tour to Delete");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetTourById_ReturnsCorrectTour()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new TourRepository(context);

            var tour = new Tour { Name = "Tour to Find", Description = "Description" };
            context.Tours.Add(tour);
            context.SaveChanges();

            // Act
            var result = repository.GetTourById(tour.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Tour to Find", result.Name);
        }

        [Fact]
        public void GetLogsForTour_ReturnsLogsForSpecificTour()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new TourRepository(context);

            var tour = new Tour { Name = "Tour with Logs" };
            var log1 = new Log { Comment = "Log 1", Tour = tour };
            var log2 = new Log { Comment = "Log 2", Tour = tour };
            context.Tours.Add(tour);
            context.Logs.AddRange(log1, log2);
            context.SaveChanges();

            // Act
            var result = repository.GetLogsForTour(tour);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, l => l.Comment == "Log 1");
            Assert.Contains(result, l => l.Comment == "Log 2");
        }

        [Fact]
        public void AddLog_AddsNewLog()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new TourRepository(context);

            var tour = new Tour { Name = "Tour for Log" };
            context.Tours.Add(tour);
            context.SaveChanges();

            var newLog = new Log { Comment = "New Log", TourId = tour.Id };

            // Act
            repository.AddLog(newLog);
            var result = context.Logs.FirstOrDefault(l => l.Comment == "New Log");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Log", result.Comment);
            Assert.Equal(tour.Id, result.TourId);
        }

        [Fact]
        public void UpdateLog_UpdatesExistingLog()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new TourRepository(context);

            var tour = new Tour { Name = "Tour for Log Update" };
            var log = new Log { Comment = "Old Log", Tour = tour };
            context.Tours.Add(tour);
            context.Logs.Add(log);
            context.SaveChanges();

            // Act
            log.Comment = "Updated Log";
            repository.UpdateLog(log);
            var result = context.Logs.FirstOrDefault(l => l.Id == log.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Log", result.Comment);
        }

        [Fact]
        public void DeleteLog_RemovesLog()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new TourRepository(context);

            var tour = new Tour { Name = "Tour for Log Deletion" };
            var log = new Log { Comment = "Log to Delete", Tour = tour };
            context.Tours.Add(tour);
            context.Logs.Add(log);
            context.SaveChanges();

            // Act
            repository.DeleteLog(log);
            var result = context.Logs.FirstOrDefault(l => l.Id == log.Id);

            // Assert
            Assert.Null(result);
        }*/
    }
    
}
