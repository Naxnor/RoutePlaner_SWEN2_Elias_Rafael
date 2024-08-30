using Microsoft.EntityFrameworkCore;
using RoutePlaner_Rafael_elias.Models;

namespace RoutePlaner_Rafael_elias.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Tour> Tours { get; set; }
        public DbSet<Log> Logs { get; set; }

        // Parameterloser Konstruktor (falls benötigt)
        public ApplicationDbContext()
        {
        }

        // Konstruktor mit DbContextOptions
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(DbManager.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tour>().ToTable("Tour");
            modelBuilder.Entity<Log>().ToTable("TourLog");

            modelBuilder.Entity<Tour>()
                .HasMany(t => t.Logs)
                .WithOne(l => l.Tour)
                .HasForeignKey(l => l.TourId);

            // Mach EncodedRoute nullable
            modelBuilder.Entity<Tour>()
                .Property(t => t.EncodedRoute)
                .IsRequired(false);

            // Seed-Daten für die Tour-Tabelle
            modelBuilder.Entity<Tour>().HasData(
                new Tour
                {
                    Id = 1,
                    Name = "TestTour",
                    Description = "Dies ist eine Test-Tour.",
                    From = "Mank",
                    To = "Wien",
                    RouteType = "driving-car",
                    StartLatitude = 48.13676667969269,
                    StartLongitude = 15.64453125,
                    EndLatitude = 48.224672649565186,
                    EndLongitude = 16.34765625,
                    EncodedRoute = string.Empty, 
                    Distance = 80.0,
                    EstimatedTime = new TimeSpan(1, 30, 0)
                }
            );
        }



    }
}

