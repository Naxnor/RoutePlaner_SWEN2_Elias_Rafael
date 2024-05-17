using Microsoft.EntityFrameworkCore;
using RoutePlaner_Rafael_elias.Models;

namespace RoutePlaner_Rafael_elias.Database
{
    public class DbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<Tour> Tours { get; set; }
        public DbSet<Log> Logs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Get the connection string from DBManager and use it to configure the database connection
            string connectionString = DbManager.GetConnection().ConnectionString;
            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}