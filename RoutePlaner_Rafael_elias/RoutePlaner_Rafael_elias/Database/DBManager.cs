using Npgsql;

namespace RoutePlaner_Rafael_elias.Database
{
    public abstract class DbManager
    {
        private const string ConnectionString = "Host=127.0.0.1;Port=5432;Username=postgres;Password=postgres;Database=routeplaner_db";
        

        public static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(ConnectionString);
        }
    }
}