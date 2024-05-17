using System.Collections.ObjectModel;
using System.Diagnostics;
using Npgsql;
using RoutePlaner_Rafael_elias.Database;
using RoutePlaner_Rafael_elias.Models;


namespace RoutePlaner_Rafael_elias.Repository
{
    public class TourRepository
    {

        public ObservableCollection<Tour> GetAllTours()
        {
            ObservableCollection<Tour> tours = new ObservableCollection<Tour>();
            string query = "SELECT * FROM \"Tour\"";

            try
            {
                using (var conn = Database.DbManager.GetConnection())
                {
                    conn.Open();
                    var cmd = new NpgsqlCommand(query, conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tours.Add(new Tour
                            {
                                Id = reader.GetInt32(
                                    reader.GetOrdinal("Tour_ID")), 
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                                    ? null
                                    : reader.GetString(reader.GetOrdinal("Description")),
                                From = reader.GetString(reader.GetOrdinal("From")),
                                To = reader.GetString(reader.GetOrdinal("To")),
                                
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load tours: {ex.Message}");
                throw; // Rethrowing the exception to be handled or logged by the calling function
            }

            return tours;
        }

        public void AddTour(Tour tour)
        {
            string commandText =
                "INSERT INTO \"Tour\" (\"Name\", \"Description\", \"From\", \"To\", \"RouteType\") VALUES (@name, @description, @from, @to, @routeType)";
            try
            {
                using (var conn = Database.DbManager.GetConnection())
                {
                    conn.Open();
                    var cmd = new NpgsqlCommand(commandText, conn);
                    cmd.Parameters.AddWithValue("@name", tour.Name);
                    cmd.Parameters.AddWithValue("@description", (object)tour.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@from", tour.From);
                    cmd.Parameters.AddWithValue("@to", tour.To);
                    cmd.Parameters.AddWithValue("@routeType", tour.RouteType);
                    cmd.Parameters.AddWithValue("@distance", tour.Distance); 
                    cmd.Parameters.AddWithValue("@estimated_time", tour.EstimatedTime); 
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error adding tour: {ex.Message}");
                throw; // Rethrowing the exception to be handled or logged by the calling function
            }
        }


        public void UpdateTour(Tour tour)
        {
            string commandText = "UPDATE \"Tour\" SET \"Name\" = @name, \"Description\" = @description, \"From\" = @from, \"To\" = @to, \"RouteType\" = @routeType WHERE \"Tour_ID\" = @id";
            try
            {
                using (var conn = Database.DbManager.GetConnection())
                {
                    conn.Open();
                    var cmd = new NpgsqlCommand(commandText, conn);
                    cmd.Parameters.AddWithValue("@name", tour.Name);
                    cmd.Parameters.AddWithValue("@description", (object)tour.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@from", tour.From);
                    cmd.Parameters.AddWithValue("@to", tour.To);
                    cmd.Parameters.AddWithValue("@routeType", tour.RouteType);
                    cmd.Parameters.AddWithValue("@id", tour.Id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating tour: {ex.Message}");
                throw; // Rethrowing the exception to be handled or logged by the calling function
            }
        }
        public string GetRouteTypeForTour(int tourId)
        {
            string routeType = null;

            string commandText = "SELECT \"RouteType\" FROM \"Tour\" WHERE \"Tour_ID\" = @tourId";
            try
            {
                using (var conn = Database.DbManager.GetConnection())
                {
                    conn.Open();
                    var cmd = new NpgsqlCommand(commandText, conn);
                    cmd.Parameters.AddWithValue("@tourId", tourId);
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        routeType = result.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving RouteType for tour ID {tourId}: {ex.Message}");
                
            }

            return routeType;
        }
        public void AddLog(Log log)
        {
            using (var connection = DbManager.GetConnection())
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = @"INSERT INTO ""TourLog"" (""Tour_ID"", ""TourDate"", ""Distance"", ""Difficulty"", ""Duration"", ""Steps"", ""Weather"", ""TotalTime"", ""Comment"", ""Rating"") 
                                        VALUES (@TourId, @TourDate, @Distance, @Difficulty, @Duration, @Steps, @Weather, @TotalTime, @Comment, @Rating)";
                    cmd.Parameters.AddWithValue("TourId", log.TourId);
                    cmd.Parameters.AddWithValue("TourDate", log.Date);
                    cmd.Parameters.AddWithValue("Distance", log.Distance);
                    cmd.Parameters.AddWithValue("Difficulty", log.Difficulty);
                    cmd.Parameters.AddWithValue("Duration", log.Duration);
                    cmd.Parameters.AddWithValue("Steps", log.Steps);
                    cmd.Parameters.AddWithValue("Weather", log.Weather);
                    cmd.Parameters.AddWithValue("TotalTime", log.TotalTime);
                    cmd.Parameters.AddWithValue("Comment", log.Comment);
                    cmd.Parameters.AddWithValue("Rating", log.Rating);
                    cmd.ExecuteNonQuery();
                }
            }
        }

    

        public void DeleteLog(Log log)
        {
            string commandText = "DELETE FROM \"TourLog\" WHERE \"TourLog_ID\" = @logId";
            try
            {
                using (var conn = Database.DbManager.GetConnection())
                {
                    conn.Open();
                    var cmd = new NpgsqlCommand(commandText, conn);
                    cmd.Parameters.AddWithValue("@logId", log.Id);
                    int result = cmd.ExecuteNonQuery();
                    if (result == 0)
                    {
                        throw new Exception("No record was deleted, check the log ID.");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting log: {ex.Message}");
                throw;
            }
        }

        public void DeleteTour(Tour tour)
        {
            string commandText = "DELETE FROM \"Tour\" WHERE \"Tour_ID\" = @id";
            try
            {
                using (var conn = Database.DbManager.GetConnection())
                {
                    conn.Open();
                    var cmd = new NpgsqlCommand(commandText, conn);
                    cmd.Parameters.AddWithValue("@id", tour.Id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting tour: {ex.Message}");
                throw; // Rethrowing the exception to be handled or logged by the calling function
            }
        }

        public void UpdateLog(Log log)
        {
            string commandText = "UPDATE \"TourLog\" SET \"TourDate\" = @date, \"Distance\" = @distance, \"Difficulty\" = @difficulty, \"Duration\" = @duration, \"Steps\" = @steps, \"Weather\" = @weather, \"TotalTime\" = @totalTime, \"Comment\" = @comment, \"Rating\" = @rating WHERE \"TourLog_ID\" = @id";
            try
            {
                using (var conn = Database.DbManager.GetConnection())
                {
                    conn.Open();
                    var cmd = new NpgsqlCommand(commandText, conn);
                    cmd.Parameters.AddWithValue("@date", log.Date);
                    cmd.Parameters.AddWithValue("@distance", log.Distance);
                    cmd.Parameters.AddWithValue("@difficulty", log.Difficulty);
                    cmd.Parameters.AddWithValue("@duration", log.Duration);
                    cmd.Parameters.AddWithValue("@steps", log.Steps);
                    cmd.Parameters.AddWithValue("@weather", log.Weather);
                    cmd.Parameters.AddWithValue("@totalTime", log.TotalTime);
                    cmd.Parameters.AddWithValue("@comment", log.Comment);
                    cmd.Parameters.AddWithValue("@rating", log.Rating);
                    cmd.Parameters.AddWithValue("@id", log.Id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating log: {ex.Message}");
                throw; // Rethrowing the exception to be handled or logged by the calling function
            }
        }
        public IEnumerable<Log> GetLogsForTour(Tour tour)
        {
            var logs = new List<Log>();
            string query = "SELECT * FROM \"TourLog\" WHERE \"Tour_ID\" = @TourId";

            try
            {
                using (var conn = Database.DbManager.GetConnection())
                {
                    conn.Open();
                    var cmd = new NpgsqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@TourId", tour.Id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var log = new Log
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("TourLog_ID")),
                                TourId = reader.GetInt32(reader.GetOrdinal("Tour_ID")),
                                Date = reader.GetDateTime(reader.GetOrdinal("TourDate")),
                                Distance = reader.GetDecimal(reader.GetOrdinal("Distance")),
                                Difficulty = reader.GetDecimal(reader.GetOrdinal("Difficulty")),
                                Duration = reader.GetDecimal(reader.GetOrdinal("Duration")), 
                                Steps = reader.GetDecimal(reader.GetOrdinal("Steps")),
                                Weather = reader.IsDBNull(reader.GetOrdinal("Weather")) ? null : reader.GetString(reader.GetOrdinal("Weather")),
                                Comment = reader.IsDBNull(reader.GetOrdinal("Comment")) ? null : reader.GetString(reader.GetOrdinal("Comment")),
                                Rating = reader.GetInt32(reader.GetOrdinal("Rating")),
                                TotalTime = reader.GetDecimal(reader.GetOrdinal("TotalTime")) 
                            };
                            logs.Add(log);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load logs for tour: {ex.Message}");
                throw;
            }

            return logs;
        }
    }
}
