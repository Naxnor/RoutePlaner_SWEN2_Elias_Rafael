using Microsoft.EntityFrameworkCore;
using RoutePlaner_Rafael_elias.Models;

namespace RoutePlaner_Rafael_elias.Database
{
    public class ApplicationDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<Tour> Tours { get; set; }
        public DbSet<Log> Logs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = DbManager.GetConnection().ConnectionString;
                optionsBuilder.UseNpgsql(connectionString);
            }
        }
        
    }
}