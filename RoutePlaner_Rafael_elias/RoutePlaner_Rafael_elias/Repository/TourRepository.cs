using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using Npgsql;
using RoutePlaner_Rafael_elias.Database;
using RoutePlaner_Rafael_elias.Models;
using Microsoft.EntityFrameworkCore;

namespace RoutePlaner_Rafael_elias.Repository
{
    public class TourRepository
    { 
        
        public ObservableCollection<Tour> GetAllTours()
        {
            using (var context = new ApplicationDbContext())
            {
                return new ObservableCollection<Tour>(context.Tours.Include(t => t.Logs).ToList());
            }
        }

        public void AddTour(Tour tour)
        {
            using (var context = new ApplicationDbContext())
            {
                try
                {
                    context.Tours.Add(tour);
                    context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    Debug.WriteLine($"An error occurred while saving the tour: {ex.InnerException?.Message ?? ex.Message}");
                    MessageBox.Show($"An error occurred while saving the tour: {ex.InnerException?.Message ?? ex.Message}");
                    throw;
                }
            }
        }




        

        public void UpdateTour(Tour tour)
        {
            using (var context = new ApplicationDbContext())
            {
                context.Tours.Update(tour);
                context.SaveChanges();  // Speichert die Änderungen in der Datenbank
            }
        }

        
        public ObservableCollection<Tour> SearchTours(string searchQuery)
        {
            using (var context = new ApplicationDbContext())
            {
                var tours = context.Tours
                    .Include(t => t.Logs)
                    .Where(t => 
                        EF.Functions.ILike(t.Name, $"%{searchQuery}%") || 
                        EF.Functions.ILike(t.Description, $"%{searchQuery}%") || 
                        t.Logs.Any(l => 
                            EF.Functions.ILike(l.Comment, $"%{searchQuery}%") || 
                            EF.Functions.ILike(l.Weather, $"%{searchQuery}%") ||
                            EF.Functions.ILike(l.Distance.ToString(), $"%{searchQuery}%") ||
                            EF.Functions.ILike(l.Duration.ToString(), $"%{searchQuery}%")
                        ))
                    .ToList();

                return new ObservableCollection<Tour>(tours);
            }
        }



        
        public int AddTourAndGetId(Tour tour)
        {
            using (var context = new ApplicationDbContext())
            {
                context.Tours.Add(tour);
                context.SaveChanges();
                return tour.Id; // Hier wird die generierte ID zurückgegeben.
            }
        }

        
        public List<Tour> GetAllToursWithLogs()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Tours.Include(t => t.Logs).ToList();
            }
        }




public void DeleteTour(Tour tour)
{
    using (var context = new ApplicationDbContext())
    {
        context.Tours.Remove(tour);
        context.SaveChanges();  // Speichert die Änderungen in der Datenbank
    }
}



public string GetRouteTypeForTour(int tourId)
{
    using (var context = new ApplicationDbContext())
    {
        return context.Tours
            .Where(t => t.Id == tourId)
            .Select(t => t.RouteType)
            .FirstOrDefault();
    }
}

        public Tour GetTourById(int tourId)
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Tours.Include(t => t.Logs).FirstOrDefault(t => t.Id == tourId);
            }
        }

        public IEnumerable<Log> GetLogsForTour(Tour tour)
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Logs.Where(l => l.TourId == tour.Id).ToList();
            }
        }


        public void AddLog(Log log)
        {
            using (var context = new ApplicationDbContext())
            {
                context.Logs.Add(log);
                context.SaveChanges();
            }
        }


        public void UpdateLog(Log log)
        {
            using (var context = new ApplicationDbContext())
            {
                context.Logs.Update(log);
                context.SaveChanges();
            }
        }


        public void DeleteLog(Log log)
        {
            using (var context = new ApplicationDbContext())
            {
                context.Logs.Remove(log);
                context.SaveChanges();
            }
        }

    }
}

