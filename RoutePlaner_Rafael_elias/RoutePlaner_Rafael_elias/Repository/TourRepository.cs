using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
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
        using (var conn = DbManager.GetConnection())
        {
            conn.Open();
            var cmd = new NpgsqlCommand(query, conn);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    tours.Add(new Tour
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Tour_ID")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                        From = reader.GetString(reader.GetOrdinal("From")),
                        To = reader.GetString(reader.GetOrdinal("To")),
                        RouteType = reader.IsDBNull(reader.GetOrdinal("RouteType")) ? null : reader.GetString(reader.GetOrdinal("RouteType")),
                        StartLatitude = reader.IsDBNull(reader.GetOrdinal("StartLatitude")) ? 0 : reader.GetDouble(reader.GetOrdinal("StartLatitude")),
                        StartLongitude = reader.IsDBNull(reader.GetOrdinal("StartLongitude")) ? 0 : reader.GetDouble(reader.GetOrdinal("StartLongitude")),
                        EndLatitude = reader.IsDBNull(reader.GetOrdinal("EndLatitude")) ? 0 : reader.GetDouble(reader.GetOrdinal("EndLatitude")),
                        EndLongitude = reader.IsDBNull(reader.GetOrdinal("EndLongitude")) ? 0 : reader.GetDouble(reader.GetOrdinal("EndLongitude")),
                        EncodedRoute = reader.IsDBNull(reader.GetOrdinal("EncodedRoute")) ? null : reader.GetString(reader.GetOrdinal("EncodedRoute"))
                    });
                }
            }
        }
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"Failed to load tours: {ex.Message}");
        throw;
    }

    return tours;
}

        public void AddTour(Tour tour)
        {
            string commandText = "INSERT INTO \"Tour\" (\"Name\", \"Description\", \"From\", \"To\", \"RouteType\", \"StartLatitude\", \"StartLongitude\", \"EndLatitude\", \"EndLongitude\") " +
                                 "VALUES (@name, @description, @from, @to, @routeType, @startLat, @startLng, @endLat, @endLng) RETURNING \"Tour_ID\"";

            try
            {
                using (var conn = DbManager.GetConnection())
                {
                    conn.Open();
                    var cmd = new NpgsqlCommand(commandText, conn);
                    cmd.Parameters.AddWithValue("name", tour.Name);
                    cmd.Parameters.AddWithValue("description", (object)tour.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("from", tour.From);
                    cmd.Parameters.AddWithValue("to", tour.To);
                    cmd.Parameters.AddWithValue("routeType", tour.RouteType);
                    cmd.Parameters.AddWithValue("startLat", tour.StartLatitude);
                    cmd.Parameters.AddWithValue("startLng", tour.StartLongitude);
                    cmd.Parameters.AddWithValue("endLat", tour.EndLatitude);
                    cmd.Parameters.AddWithValue("endLng", tour.EndLongitude);

                    // Tour_ID nach dem Einfügen zurückgeben und dem Tour-Objekt zuweisen
                    tour.Id = (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error adding tour: {ex.Message}");
                throw;
            }
        }




        public void UpdateTour(Tour tour)
        {
            string commandText = "UPDATE \"Tour\" SET \"Name\" = @name, \"Description\" = @description, \"From\" = @from, \"To\" = @to, \"RouteType\" = @routeType, " +
                                 "\"StartLatitude\" = @startLat, \"StartLongitude\" = @startLng, \"EndLatitude\" = @endLat, \"EndLongitude\" = @endLng, \"EncodedRoute\" = @encodedRoute, " +
                                 "\"Distance\" = @distance, \"EstimatedTime\" = @estimatedTime " +
                                 "WHERE \"Tour_ID\" = @id";

            try
            {
                using (var conn = DbManager.GetConnection())
                {
                    conn.Open();
                    var cmd = new NpgsqlCommand(commandText, conn);
                    cmd.Parameters.AddWithValue("name", tour.Name);
                    cmd.Parameters.AddWithValue("description", (object)tour.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("from", tour.From);
                    cmd.Parameters.AddWithValue("to", tour.To);
                    cmd.Parameters.AddWithValue("routeType", tour.RouteType);
                    cmd.Parameters.AddWithValue("startLat", tour.StartLatitude);
                    cmd.Parameters.AddWithValue("startLng", tour.StartLongitude);
                    cmd.Parameters.AddWithValue("endLat", tour.EndLatitude);
                    cmd.Parameters.AddWithValue("endLng", tour.EndLongitude);
                    cmd.Parameters.AddWithValue("encodedRoute", (object)tour.EncodedRoute ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("distance", tour.Distance);
                    cmd.Parameters.AddWithValue("estimatedTime", tour.EstimatedTime);
                    cmd.Parameters.AddWithValue("id", tour.Id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating tour: {ex.Message}");
                throw;
            }
        }
        public ObservableCollection<Tour> SearchTours(string searchQuery)
{
    ObservableCollection<Tour> tours = new ObservableCollection<Tour>();
    string query = @"
        SELECT t.*, l.*
        FROM ""Tour"" t
        LEFT JOIN ""TourLog"" l ON t.""Tour_ID"" = l.""Tour_ID""
        WHERE 
            t.""Name"" ILIKE @query OR 
            t.""Description"" ILIKE @query OR
            l.""Comment"" ILIKE @query OR
            l.""Weather"" ILIKE @query OR
            CAST(l.""Distance"" AS TEXT) ILIKE @query OR
            CAST(l.""Duration"" AS TEXT) ILIKE @query";

    try
    {
        using (var conn = DbManager.GetConnection())
        {
            conn.Open();
            var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("query", $"%{searchQuery}%");

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    // Prüfen, ob die Tour bereits in der Sammlung ist
                    var tour = tours.FirstOrDefault(t => t.Id == reader.GetInt32(reader.GetOrdinal("Tour_ID")));
                    if (tour == null)
                    {
                        // Neue Tour hinzufügen, wenn noch nicht vorhanden
                        tour = new Tour
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Tour_ID")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                            From = reader.GetString(reader.GetOrdinal("From")),
                            To = reader.GetString(reader.GetOrdinal("To")),
                            RouteType = reader.IsDBNull(reader.GetOrdinal("RouteType")) ? null : reader.GetString(reader.GetOrdinal("RouteType")),
                            StartLatitude = reader.IsDBNull(reader.GetOrdinal("StartLatitude")) ? 0 : reader.GetDouble(reader.GetOrdinal("StartLatitude")),
                            StartLongitude = reader.IsDBNull(reader.GetOrdinal("StartLongitude")) ? 0 : reader.GetDouble(reader.GetOrdinal("StartLongitude")),
                            EndLatitude = reader.IsDBNull(reader.GetOrdinal("EndLatitude")) ? 0 : reader.GetDouble(reader.GetOrdinal("EndLatitude")),
                            EndLongitude = reader.IsDBNull(reader.GetOrdinal("EndLongitude")) ? 0 : reader.GetDouble(reader.GetOrdinal("EndLongitude")),
                            EncodedRoute = reader.IsDBNull(reader.GetOrdinal("EncodedRoute")) ? null : reader.GetString(reader.GetOrdinal("EncodedRoute"))
                        };
                        tours.Add(tour);
                    }

                    // Log zur existierenden Tour hinzufügen
                    if (!reader.IsDBNull(reader.GetOrdinal("TourLog_ID")))
                    {
                        tour.Logs.Add(new Log
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("TourLog_ID")),
                            TourId = reader.GetInt32(reader.GetOrdinal("Tour_ID")),
                            Date = reader.GetDateTime(reader.GetOrdinal("TourDate")),
                            Distance = reader.IsDBNull(reader.GetOrdinal("Distance")) ? 0 : reader.GetDecimal(reader.GetOrdinal("Distance")),
                            Difficulty = reader.IsDBNull(reader.GetOrdinal("Difficulty")) ? 0 : reader.GetDecimal(reader.GetOrdinal("Difficulty")),
                            Duration = reader.IsDBNull(reader.GetOrdinal("Duration")) ? 0 : reader.GetDecimal(reader.GetOrdinal("Duration")),
                            Steps = reader.IsDBNull(reader.GetOrdinal("Steps")) ? 0 : reader.GetDecimal(reader.GetOrdinal("Steps")),
                            Weather = reader.IsDBNull(reader.GetOrdinal("Weather")) ? null : reader.GetString(reader.GetOrdinal("Weather")),
                            Comment = reader.IsDBNull(reader.GetOrdinal("Comment")) ? null : reader.GetString(reader.GetOrdinal("Comment")),
                            Rating = reader.GetInt32(reader.GetOrdinal("Rating")),
                            TotalTime = reader.IsDBNull(reader.GetOrdinal("TotalTime")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TotalTime"))
                        });
                    }
                }
            }
        }
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"Error during search: {ex.Message}");
        throw;
    }

    return tours;
}


        
        public int AddTourAndGetId(Tour tour)
        {
            string commandText = "INSERT INTO \"Tour\" (\"Name\", \"Description\", \"From\", \"To\", \"RouteType\", \"StartLatitude\", \"StartLongitude\", \"EndLatitude\", \"EndLongitude\", \"Distance\", \"EstimatedTime\") " +
                                 "VALUES (@name, @description, @from, @to, @routeType, @startLat, @startLng, @endLat, @endLng, @distance, @estimatedTime) RETURNING \"Tour_ID\"";

            try
            {
                using (var conn = DbManager.GetConnection())
                {
                    conn.Open();
                    var cmd = new NpgsqlCommand(commandText, conn);
                    cmd.Parameters.AddWithValue("name", tour.Name);
                    cmd.Parameters.AddWithValue("description", (object)tour.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("from", tour.From);
                    cmd.Parameters.AddWithValue("to", tour.To);
                    cmd.Parameters.AddWithValue("routeType", tour.RouteType);
                    cmd.Parameters.AddWithValue("startLat", tour.StartLatitude);
                    cmd.Parameters.AddWithValue("startLng", tour.StartLongitude);
                    cmd.Parameters.AddWithValue("endLat", tour.EndLatitude);
                    cmd.Parameters.AddWithValue("endLng", tour.EndLongitude);
                    cmd.Parameters.AddWithValue("distance", tour.Distance);
                    cmd.Parameters.AddWithValue("estimatedTime", tour.EstimatedTime);

                    // Execute and get the newly inserted Tour_ID
                    return (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error adding tour and getting ID: {ex.Message}");
                throw;
            }
        }
public List<Tour> GetAllToursWithLogs()
{
    var tours = new List<Tour>();
    string query = "SELECT * FROM \"Tour\"";

    try
    {
        using (var conn = DbManager.GetConnection())
        {
            conn.Open();
            var cmd = new NpgsqlCommand(query, conn);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var tour = new Tour
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Tour_ID")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                        From = reader.GetString(reader.GetOrdinal("From")),
                        To = reader.GetString(reader.GetOrdinal("To")),
                        RouteType = reader.IsDBNull(reader.GetOrdinal("RouteType")) ? null : reader.GetString(reader.GetOrdinal("RouteType")),
                        StartLatitude = reader.IsDBNull(reader.GetOrdinal("StartLatitude")) ? 0 : reader.GetDouble(reader.GetOrdinal("StartLatitude")),
                        StartLongitude = reader.IsDBNull(reader.GetOrdinal("StartLongitude")) ? 0 : reader.GetDouble(reader.GetOrdinal("StartLongitude")),
                        EndLatitude = reader.IsDBNull(reader.GetOrdinal("EndLatitude")) ? 0 : reader.GetDouble(reader.GetOrdinal("EndLatitude")),
                        EndLongitude = reader.IsDBNull(reader.GetOrdinal("EndLongitude")) ? 0 : reader.GetDouble(reader.GetOrdinal("EndLongitude")),
                        EncodedRoute = reader.IsDBNull(reader.GetOrdinal("EncodedRoute")) ? null : reader.GetString(reader.GetOrdinal("EncodedRoute")),
                        Distance = reader.IsDBNull(reader.GetOrdinal("Distance")) ? 0 : reader.GetDouble(reader.GetOrdinal("Distance")),
                        EstimatedTime = reader.IsDBNull(reader.GetOrdinal("EstimatedTime")) ? TimeSpan.Zero : reader.GetTimeSpan(reader.GetOrdinal("EstimatedTime"))
                    };

                    // Fetch logs for each tour
                    tour.Logs = GetLogsForTour(tour).ToList();
                    tours.Add(tour);
                }
            }
        }
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"Failed to load tours with logs: {ex.Message}");
        throw;
    }

    return tours;
}



public void DeleteTour(Tour tour)
{
    string commandText = "DELETE FROM \"Tour\" WHERE \"Tour_ID\" = @id";

    try
    {
        using (var conn = DbManager.GetConnection())
        {
            conn.Open();
            var cmd = new NpgsqlCommand(commandText, conn);
            cmd.Parameters.AddWithValue("id", tour.Id);

            // Überprüfen, ob eine Zeile gelöscht wurde
            int rowsAffected = cmd.ExecuteNonQuery();
            if (rowsAffected == 0)
            {
                throw new Exception("Tour not deleted. Tour ID not found.");
            }
        }
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"Error deleting tour: {ex.Message}");
        throw;
    }
}


        public string GetRouteTypeForTour(int tourId)
        {
            string routeType = null;
            string commandText = "SELECT \"RouteType\" FROM \"Tour\" WHERE \"Tour_ID\" = @tourId";

            try
            {
                using (var conn = DbManager.GetConnection())
                {
                    conn.Open();
                    var cmd = new NpgsqlCommand(commandText, conn);
                    cmd.Parameters.AddWithValue("tourId", tourId);
                    var result = cmd.ExecuteScalar();
                    routeType = result != null && result != DBNull.Value ? result.ToString() : null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving RouteType for tour ID {tourId}: {ex.Message}");
            }

            return routeType;
        }
public Tour GetTourById(int tourId)
        {
            Tour tour = null;
            string query = "SELECT * FROM \"Tour\" WHERE \"Tour_ID\" = @tourId";

            try
            {
                using (var conn = DbManager.GetConnection())
                {
                    conn.Open();
                    var cmd = new NpgsqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("tourId", tourId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            tour = new Tour
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Tour_ID")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                From = reader.GetString(reader.GetOrdinal("From")),
                                To = reader.GetString(reader.GetOrdinal("To")),
                                RouteType = reader.IsDBNull(reader.GetOrdinal("RouteType")) ? null : reader.GetString(reader.GetOrdinal("RouteType")),
                                StartLatitude = reader.IsDBNull(reader.GetOrdinal("StartLatitude")) ? 0 : reader.GetDouble(reader.GetOrdinal("StartLatitude")),
                                StartLongitude = reader.IsDBNull(reader.GetOrdinal("StartLongitude")) ? 0 : reader.GetDouble(reader.GetOrdinal("StartLongitude")),
                                EndLatitude = reader.IsDBNull(reader.GetOrdinal("EndLatitude")) ? 0 : reader.GetDouble(reader.GetOrdinal("EndLatitude")),
                                EndLongitude = reader.IsDBNull(reader.GetOrdinal("EndLongitude")) ? 0 : reader.GetDouble(reader.GetOrdinal("EndLongitude")),
                                EncodedRoute = reader.IsDBNull(reader.GetOrdinal("EncodedRoute")) ? null : reader.GetString(reader.GetOrdinal("EncodedRoute")),
                                Distance = reader.IsDBNull(reader.GetOrdinal("Distance")) ? 0 : reader.GetDouble(reader.GetOrdinal("Distance")),
                                EstimatedTime = reader.IsDBNull(reader.GetOrdinal("EstimatedTime")) ? TimeSpan.Zero : reader.GetTimeSpan(reader.GetOrdinal("EstimatedTime"))
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching tour details: {ex.Message}");
                throw;
            }

            return tour;
        }
        public IEnumerable<Log> GetLogsForTour(Tour tour)
        {
            var logs = new List<Log>();
            string query = "SELECT * FROM \"TourLog\" WHERE \"Tour_ID\" = @TourId";

            try
            {
                using (var conn = DbManager.GetConnection())
                {
                    conn.Open();
                    var cmd = new NpgsqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("TourId", tour.Id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            logs.Add(new Log
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
                            });
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

        public void AddLog(Log log)
        {
            string commandText = @"INSERT INTO ""TourLog"" (""Tour_ID"", ""TourDate"", ""Distance"", ""Difficulty"", ""Duration"", ""Steps"", ""Weather"", ""TotalTime"", ""Comment"", ""Rating"") 
                                   VALUES (@TourId, @TourDate, @Distance, @Difficulty, @Duration, @Steps, @Weather, @TotalTime, @Comment, @Rating)";

            try
            {
                using (var connection = DbManager.GetConnection())
                {
                    connection.Open();
                    var cmd = new NpgsqlCommand(commandText, connection);
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
            catch (Exception ex)
            {
                Debug.WriteLine($"Error adding log: {ex.Message}");
                throw;
            }
        }

        public void UpdateLog(Log log)
        {
            string commandText = "UPDATE \"TourLog\" SET \"TourDate\" = @date, \"Distance\" = @distance, \"Difficulty\" = @difficulty, \"Duration\" = @duration, " +
                                 "\"Steps\" = @steps, \"Weather\" = @weather, \"TotalTime\" = @totalTime, \"Comment\" = @comment, \"Rating\" = @rating " +
                                 "WHERE \"TourLog_ID\" = @id";

            try
            {
                using (var conn = DbManager.GetConnection())
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
                throw;
            }
        }

        public void DeleteLog(Log log)
        {
            string commandText = "DELETE FROM \"TourLog\" WHERE \"TourLog_ID\" = @logId";

            try
            {
                using (var conn = DbManager.GetConnection())
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
    }
}

