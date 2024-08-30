using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using RoutePlaner_Rafael_elias.Database;
using RoutePlaner_Rafael_elias.Models;

namespace RoutePlaner_Rafael_elias.Repository
{
    public class TourRepository
    {
        private readonly ApplicationDbContext _context;

        public TourRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        
        // Parameterloser Konstruktor
        public TourRepository()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(DbManager.ConnectionString)
                .Options;

            _context = new ApplicationDbContext(options);
        }

        public ObservableCollection<Tour> GetAllTours()
        {
            return new ObservableCollection<Tour>(_context.Tours.Include(t => t.Logs).ToList());
        }

        public void AddTour(Tour tour)
        {
            try
            {
                _context.Tours.Add(tour);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                Debug.WriteLine($"An error occurred while saving the tour: {ex.InnerException?.Message ?? ex.Message}");
                MessageBox.Show($"An error occurred while saving the tour: {ex.InnerException?.Message ?? ex.Message}");
                throw;
            }
        }

        public void UpdateTour(Tour tour)
        {
            _context.Tours.Update(tour);
            _context.SaveChanges();  // Speichert die Änderungen in der Datenbank
        }

        public void DeleteTour(Tour tour)
        {
            _context.Tours.Remove(tour);
            _context.SaveChanges();  // Speichert die Änderungen in der Datenbank
        }

        public ObservableCollection<Tour> SearchTours(string searchQuery)
        {
            var tours = _context.Tours
                .Include(t => t.Logs)
                .Where(t => 
                    t.Name.ToLower().Contains(searchQuery.ToLower()) || 
                    t.Description.ToLower().Contains(searchQuery.ToLower()) || 
                    t.Logs.Any(l => 
                        l.Comment.ToLower().Contains(searchQuery.ToLower()) || 
                        l.Weather.ToLower().Contains(searchQuery.ToLower()) ||
                        l.Distance.ToString().Contains(searchQuery) ||
                        l.Duration.ToString().Contains(searchQuery)
                    ))
                .ToList();

            return new ObservableCollection<Tour>(tours);
        }


        public int AddTourAndGetId(Tour tour)
        {
            _context.Tours.Add(tour);
            _context.SaveChanges();
            return tour.Id; // Hier wird die generierte ID zurückgegeben.
        }

        public List<Tour> GetAllToursWithLogs()
        {
            return _context.Tours.Include(t => t.Logs).ToList();
        }

        public string GetRouteTypeForTour(int tourId)
        {
            return _context.Tours
                .Where(t => t.Id == tourId)
                .Select(t => t.RouteType)
                .FirstOrDefault();
        }

        public Tour GetTourById(int tourId)
        {
            return _context.Tours.Include(t => t.Logs).FirstOrDefault(t => t.Id == tourId);
        }

        public IEnumerable<Log> GetLogsForTour(Tour tour)
        {
            return _context.Logs.Where(l => l.TourId == tour.Id).ToList();
        }

        public void AddLog(Log log)
        {
            _context.Logs.Add(log);
            _context.SaveChanges();
        }

        public void UpdateLog(Log log)
        {
            _context.Logs.Update(log);
            _context.SaveChanges();
        }

        public void DeleteLog(Log log)
        {
            _context.Logs.Remove(log);
            _context.SaveChanges();
        }
    }
}
