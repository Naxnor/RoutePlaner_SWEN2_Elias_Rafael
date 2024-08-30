using Microsoft.EntityFrameworkCore;
using RoutePlaner_Rafael_elias.Models;

namespace RoutePlaner_Rafael_elias.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Tour> Tours { get; set; }
        public DbSet<Log> Logs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=127.0.0.1;Port=5432;Username=postgres;Password=postgres;Database=postgres");
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
        }
    }
}

