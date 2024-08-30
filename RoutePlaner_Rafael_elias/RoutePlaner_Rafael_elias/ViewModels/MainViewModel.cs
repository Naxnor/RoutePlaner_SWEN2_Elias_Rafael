using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using RoutePlaner_Rafael_elias.Models;
using RoutePlaner_Rafael_elias.Repository;
using RoutePlaner_Rafael_elias.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using log4net;
using TextAlignment = iText.Layout.Properties.TextAlignment;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.Linq;
using Microsoft.Win32;
using Newtonsoft.Json;
using RoutePlaner_Rafael_elias.Database;

namespace RoutePlaner_Rafael_elias.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MainViewModel));

        private readonly TourRepository _repository;
        private readonly RouteService _routeService;
        public ICommand ExportTourDataCommand { get; }
        public ICommand ImportTourDataCommand { get; }
        public ICommand GenerateSummarizedReportCommand { get; }
        public ICommand GenerateTourReportCommand { get; }
        private Tour _selectedTour;
        private Log _selectedLog;
        private WebBrowser _mapBrowser;
        private ObservableCollection<Log> _dataGridLogList;
        private string _selectedTourDescription;
        private string _imagePath = @"F:\GIT\SWEN\RoutePlaner_Rafael_elias\RoutePlaner_Rafael_elias\Data\Images\image.png";
        public string SearchQuery { get; set; }
        
        public ICommand FetchRouteCommand { get; }
        public ICommand ShowMapCommand { get; }
        public ICommand OpenAddTourWindowCommand { get; private set; }
        public ICommand OpenUpdateTourWindowCommand { get; private set; }
        public ICommand DeleteTourCommand { get; private set; }
        public ICommand AddLogCommand { get; private set; }
        public ICommand UpdateLogCommand { get; private set; }
        public ICommand DeleteLogCommand { get; private set; }
        public ICommand GenerateReportCommand { get; private set; }
        public ICommand GenerateLogReportCommand { get; private set; }
        public ICommand ExecuteSearchCommand { get; }

        public MainViewModel()
        {
            _repository = new TourRepository();
            _routeService = new RouteService();
            GenerateTourReportCommand = new RelayCommand<Tour>(GenerateSingleTourReport);
            GenerateSummarizedReportCommand = new RelayCommand(GenerateSummarizedReport);
            FetchRouteCommand = new AsyncRelayCommand(FetchRouteData);
            ShowMapCommand = new RelayCommand(ShowMap);
            GenerateReportCommand = new RelayCommand(GenerateTourReport);
            GenerateLogReportCommand = new RelayCommand(GenerateLogReport);
            ExportTourDataCommand = new RelayCommand(ExportTourData);
            ImportTourDataCommand = new RelayCommand(ImportTourData);
            Tours = new ObservableCollection<Tour>();
            DataGridLogList = new ObservableCollection<Log>();
            ExecuteSearchCommand = new RelayCommand(ExecuteSearch);

            InitializeCommands();
            LoadTours();

            WeakReferenceMessenger.Default.Register<LogUpdatedMessage>(this, (r, m) =>
            {
                LoadLogs();
            });
        }

        public ObservableCollection<Tour> Tours { get; private set; }

        public ObservableCollection<Log> DataGridLogList
        {
            get => _dataGridLogList;
            set => SetProperty(ref _dataGridLogList, value);
        }
        public void GenerateSummarizedReport()
        {
            try
            {
                var allTours = _repository.GetAllToursWithLogs();

                string pdfPath = "SummarizedReport_AllTours.pdf";

                using (PdfWriter writer = new PdfWriter(pdfPath))
                {
                    using (PdfDocument pdfDoc = new PdfDocument(writer))
                    {
                        Document document = new Document(pdfDoc);

                        document.Add(new Paragraph("Summarized Tour Report"));
                        document.Add(new Paragraph(""));

                        foreach (var tour in allTours)
                        {
                            // Add Tour Details
                            document.Add(new Paragraph($"Tour: {tour.Name}"));
                            document.Add(new Paragraph($"From: {tour.From} To: {tour.To}"));
                            document.Add(new Paragraph($"Description: {tour.Description}"));
                            document.Add(new Paragraph($"Route Type: {tour.RouteType}"));
                            document.Add(new Paragraph($"Distance: {tour.Distance:F3} km")); // Display up to 3 decimal places
                            document.Add(new Paragraph($"Estimated Time: {tour.EstimatedTime}"));
                            document.Add(new Paragraph(""));

                            // Add Logs
                            document.Add(new Paragraph("Logs:"));
                            if (tour.Logs != null && tour.Logs.Any())
                            {
                                foreach (var log in tour.Logs)
                                {
                                    document.Add(new Paragraph($"Log ID: {log.Id}"));
                                    document.Add(new Paragraph($"Date: {log.Date:yyyy-MM-dd HH:mm:ss}"));
                                    document.Add(new Paragraph($"Distance: {log.Distance} km"));
                                    document.Add(new Paragraph($"Difficulty: {log.Difficulty}"));
                                    document.Add(new Paragraph($"Duration: {log.Duration} hrs"));
                                    document.Add(new Paragraph($"Steps: {log.Steps}"));
                                    document.Add(new Paragraph($"Weather: {log.Weather}"));
                                    document.Add(new Paragraph($"Comment: {log.Comment}"));
                                    document.Add(new Paragraph($"Rating: {log.Rating}"));
                                    document.Add(new Paragraph(""));
                                }
                            }
                            else
                            {
                                document.Add(new Paragraph("No logs available for this tour."));
                            }

                            document.Add(new Paragraph("\n--------------------------------------------------\n"));
                        }

                        document.Close();
                    }
                }

                MessageBox.Show($"Summarized report generated successfully at {pdfPath}!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error generating summarized PDF report: {ex.Message}");
                MessageBox.Show($"Error generating summarized PDF report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
public void GenerateSingleTourReport(Tour tour)
        {
            if (tour == null)
            {
                MessageBox.Show("Please select a tour to generate the report.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Fetch the most recent tour details from the database
                var updatedTour = _repository.GetTourById(tour.Id);
                if (updatedTour == null)
                {
                    MessageBox.Show("Tour not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Fetch logs for the updated tour
                var logs = _repository.GetLogsForTour(updatedTour).ToList();

                string pdfPath = $"TourReport_{updatedTour.Name.Replace(" ", "_")}.pdf";

                using (PdfWriter writer = new PdfWriter(pdfPath))
                {
                    using (PdfDocument pdfDoc = new PdfDocument(writer))
                    {
                        Document document = new Document(pdfDoc);

                        // Add Tour Details from the database
                        document.Add(new Paragraph($"Tour Report: {updatedTour.Name}"));
                        document.Add(new Paragraph($"From: {updatedTour.From} To: {updatedTour.To}"));
                        document.Add(new Paragraph($"Description: {updatedTour.Description}"));
                        document.Add(new Paragraph($"Route Type: {updatedTour.RouteType}"));
                        document.Add(new Paragraph($"Distance: {updatedTour.Distance:F3} km"));  // Display up to 3 decimal places
                        document.Add(new Paragraph($"Estimated Time: {updatedTour.EstimatedTime}"));

                        // Add Logs
                        document.Add(new Paragraph("\nLogs:"));
                        foreach (var log in logs)
                        {
                            document.Add(new Paragraph($"Log ID: {log.Id}"));
                            document.Add(new Paragraph($"Date: {log.Date:yyyy-MM-dd HH:mm:ss}"));
                            document.Add(new Paragraph($"Distance: {log.Distance} km"));
                            document.Add(new Paragraph($"Difficulty: {log.Difficulty}"));
                            document.Add(new Paragraph($"Duration: {log.Duration} hrs"));
                            document.Add(new Paragraph($"Steps: {log.Steps}"));
                            document.Add(new Paragraph($"Weather: {log.Weather}"));
                            document.Add(new Paragraph($"Comment: {log.Comment}"));
                            document.Add(new Paragraph($"Rating: {log.Rating}"));
                            document.Add(new Paragraph("\n"));
                        }

                        document.Close();
                    }
                }

                MessageBox.Show($"Tour report generated successfully at {pdfPath}!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error generating PDF report: {ex.Message}");
                MessageBox.Show($"Error generating PDF report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public string ImagePath
        {
            get => _imagePath;
            set => SetProperty(ref _imagePath, value);
        }

        public WebBrowser MapBrowser
        {
            get => _mapBrowser;
            set => SetProperty(ref _mapBrowser, value);
        }

        public string SelectedTourDescription
        {
            get => _selectedTourDescription;
            set => SetProperty(ref _selectedTourDescription, value);
        }

        public Log SelectedLog
        {
            get => _selectedLog;
            set
            {
                if (SetProperty(ref _selectedLog, value))
                {
                    ((RelayCommand)UpdateLogCommand).NotifyCanExecuteChanged();
                    ((RelayCommand)DeleteLogCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public Tour SelectedTour
        {
            get => _selectedTour;
            set
            {
                if (SetProperty(ref _selectedTour, value))
                {
                    SelectedTourDescription = value?.Description;
                    LoadLogs();

                    ((RelayCommand<Tour>)GenerateTourReportCommand).NotifyCanExecuteChanged(); 
                    ((RelayCommand)AddLogCommand).NotifyCanExecuteChanged();
                    ((RelayCommand)OpenUpdateTourWindowCommand).NotifyCanExecuteChanged();
                    ((RelayCommand)DeleteTourCommand).NotifyCanExecuteChanged();
                }
            }
        }

      private async Task FetchRouteData()
{
    try
    {
        if (SelectedTour == null)
        {
            MessageBox.Show("Please select a tour first.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        // Fetch route data using the route service
        var routeData = await _routeService.GetDirectionsAsync(
            SelectedTour.StartLatitude,
            SelectedTour.StartLongitude,
            SelectedTour.EndLatitude,
            SelectedTour.EndLongitude,
            SelectedTour.RouteType
        );

        if (routeData == null)
        {
            MessageBox.Show("Failed to fetch route data. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            log.Error("Failed to fetch route data: routeData is null.");
            return;
        }

        Debug.WriteLine($"Received route data: Distance = {routeData.Distance}, Duration = {routeData.Duration}, EncodedPolyline = {routeData.EncodedPolyline}");

        // Convert distance from meters to kilometers and round to 3 decimal places
        SelectedTour.Distance = Math.Round(routeData.Distance / 1000.0, 3);

        // Convert duration from seconds to TimeSpan
        SelectedTour.EstimatedTime = TimeSpan.FromSeconds(routeData.Duration);

        // Store encoded route
        SelectedTour.EncodedRoute = routeData.EncodedPolyline;

        Debug.WriteLine($"Encoded Route: {SelectedTour.EncodedRoute}");

        // Update the tour in the database
        try
        {
            _repository.UpdateTour(SelectedTour);  // Ensure your repository method correctly updates the Tour
            log.Info("Tour updated successfully in the database.");
        }
        catch (Exception ex)
        {
            log.Error($"Error updating tour in database: {ex.Message}", ex);
            MessageBox.Show($"Error updating tour in database: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        ShowMap(); // Attempt to display the map
        log.Info("Route data fetched and tour updated successfully.");
        MessageBox.Show("Route data fetched and updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }
    catch (Exception ex)
    {
        log.Error($"Error fetching route data: {ex.Message}", ex);
        MessageBox.Show($"Error fetching route data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}



        public void ShowMap()
        {
            if (MapBrowser == null || string.IsNullOrEmpty(SelectedTour?.EncodedRoute))
            {
                //FEHLER
                //MessageBox.Show("MapBrowser control is not set or route is not available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Decode the encoded route
            var decodedCoordinates = PolylineDecoder.DecodePolyline(SelectedTour.EncodedRoute);
            if (decodedCoordinates.Count == 0)
            {
                MessageBox.Show("No route coordinates available to display.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Extract the start and end coordinates
            var startCoordinate = decodedCoordinates.First();
            var endCoordinate = decodedCoordinates.Last();

            // Calculate the center and zoom level (simple example, you might want a more accurate calculation)
            var centerLat = (startCoordinate.Latitude + endCoordinate.Latitude) / 2;
            var centerLon = (startCoordinate.Longitude + endCoordinate.Longitude) / 2;

            // Construct the OpenStreetMap URL
            string url = $"https://www.openstreetmap.org/directions?engine=fossgis_osrm_bike&route={startCoordinate.Latitude}%2C{startCoordinate.Longitude}%3B{endCoordinate.Latitude}%2C{endCoordinate.Longitude}#map=12/{centerLat}/{centerLon}";

            // Log the URL for debugging
            Debug.WriteLine($"Generated OpenStreetMap URL: {url}");

            // Navigate the WebBrowser control to the OpenStreetMap URL
            try
            {
                MapBrowser.Navigate(url);
                log.Info("Map displayed successfully.");
            }
            catch (Exception ex)
            {
                log.Error($"Error navigating to URL: {ex.Message}", ex);
                MessageBox.Show($"Error navigating to URL: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializeCommands()
        {
            OpenAddTourWindowCommand = new RelayCommand(OpenAddTourWindow);
            OpenUpdateTourWindowCommand = new RelayCommand(OpenUpdateTourWindow, CanModifyTour);
            DeleteTourCommand = new RelayCommand(DeleteTour, CanModifyTour);
            AddLogCommand = new RelayCommand(OpenAddLogWindow, CanModifyTour);
            UpdateLogCommand = new RelayCommand(OpenUpdateLogWindow, CanModifyLog);
            DeleteLogCommand = new RelayCommand(DeleteLog, CanModifyLog);
        }

        private bool CanModifyTour()
        {
            return SelectedTour != null;
        }

        private bool CanModifyLog()
        {
            return SelectedLog != null;
        }

        private void OpenAddTourWindow()
        {
            var window = new AddTourWindow();
            window.DataContext = new AddTourViewModel();
            window.ShowDialog();
            LoadTours();  // Reload tours after adding a new one
        }

        private void OpenUpdateTourWindow()
        {
            if (SelectedTour != null)
            {
                var window = new UpdateTourWindow(SelectedTour);
                window.ShowDialog();
                LoadTours();  // Reload tours after updating
            }
        }

        public void ExportTourData()
        {
            try
            {
                // Fetch all tours with logs
                var allTours = _repository.GetAllToursWithLogs();

                // Configure serializer settings to handle self-referencing loops
                var settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                // Serialize to JSON
                string json = JsonConvert.SerializeObject(allTours, settings);

                // Define file path
                string filePath = "ToursDataExport.json";

                // Write to file
                File.WriteAllText(filePath, json);

                MessageBox.Show($"Tour data exported successfully to {filePath}!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting tour data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public void ImportTourData()
{
    try
    {
        // Open file dialog to choose a .json file
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
        if (openFileDialog.ShowDialog() != true)
        {
            return; // User cancelled the file dialog
        }

        string filePath = openFileDialog.FileName;

        // Read from the selected file
        string json = File.ReadAllText(filePath);

        // Deserialize JSON to tour objects
        var importedTours = JsonConvert.DeserializeObject<List<Tour>>(json);

        // Import each tour and its logs
        foreach (var tour in importedTours)
        {
            using (var context = new ApplicationDbContext())
            {
                try
                {
                    // Insert tour and get new ID
                    context.Tours.Add(tour);
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving tour '{tour.Name}': {ex.Message}\nDetails: {ex.InnerException?.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Update TourId for each log and insert it
                foreach (var log in tour.Logs)
                {
                    try
                    {
                        log.TourId = tour.Id; // Ensure correct TourId
                        context.Logs.Add(log);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving log for tour '{tour.Name}': {ex.Message}\nDetails: {ex.InnerException?.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                try
                {
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving logs for tour '{tour.Name}': {ex.Message}\nDetails: {ex.InnerException?.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        MessageBox.Show($"Tour data imported successfully from {filePath}!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error importing tour data: {ex.Message}\nDetails: {ex.InnerException?.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}


        
        private void OpenAddLogWindow()
        {
            var addLogWindow = new AddLogWindow();
            var addLogViewModel = new AddLogViewModel();

            if (SelectedTour != null)
            {
                addLogViewModel.LogAddUtility.TourId = SelectedTour.Id;
            }

            addLogWindow.DataContext = addLogViewModel;
            addLogWindow.ShowDialog();
            LoadLogs();  // Reload logs after adding a new one
        }

        private void OpenUpdateLogWindow()
        {
            if (SelectedLog != null)
            {
                var window = new UpdateLogWindow(SelectedLog);
                window.ShowDialog();
                LoadLogs();  // Reload logs after updating
            }
        }

        private void DeleteTour()
        {
            if (SelectedTour != null)
            {
                var result = MessageBox.Show($"Are you sure you want to delete the tour '{SelectedTour.Name}'?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        log.Info($"Attempting to delete tour: {SelectedTour.Name}");
                
                        // Lösche die Tour aus der Datenbank
                        _repository.DeleteTour(SelectedTour);
                
                        // Entferne die Tour aus der Liste
                        Tours.Remove(SelectedTour);
                
                        // Leere die Log-Anzeige, da die Logs zur gelöschten Tour gehören
                        DataGridLogList.Clear();
                
                        // Setze die ausgewählte Tour auf null
                        SelectedTour = null;
                
                        MessageBox.Show("Tour deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        log.Info($"Tour deleted successfully.");
                    }
                    catch (Exception ex)
                    {
                        log.Error($"Error deleting tour: {ex.Message}", ex);
                        MessageBox.Show($"Error deleting tour: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                log.Warn("Delete command was triggered, but no tour was selected.");
                MessageBox.Show("No tour selected for deletion.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void DeleteLog()
        {
            if (SelectedLog == null)
            {
                MessageBox.Show("Please select a log to delete.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete this log?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _repository.DeleteLog(SelectedLog);
                    DataGridLogList.Remove(SelectedLog);
                    SelectedLog = null; // Clear the selection
                    MessageBox.Show("Log deleted successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    log.Info("Log deleted successfully.");
                }
                catch (Exception ex)
                {
                    log.Error($"Error deleting log: {ex.Message}", ex);
                    MessageBox.Show($"Error deleting log: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LoadTours()
        {
            try
            {
                Tours.Clear();
                var tours = _repository.GetAllTours();
                foreach (var tour in tours)
                {
                    Tours.Add(tour);
                }
                log.Info("Tours loaded successfully.");
            }
            catch (Exception ex)
            {
                log.Error($"Failed to load tours: {ex.Message}", ex);
                Debug.WriteLine("Failed to load tours: " + ex.Message);
            }
        }

        private void LoadLogs()
        {
            try
            {
                if (SelectedTour != null)
                {
                    DataGridLogList = new ObservableCollection<Log>(_repository.GetLogsForTour(SelectedTour));
                    Debug.WriteLine($"Number of logs loaded for tour {SelectedTour.Name}: {DataGridLogList.Count}");
                    log.Info($"Logs loaded for tour {SelectedTour.Name}.");
                }
            }
            catch (Exception ex)
            {
                log.Error($"Failed to load logs: {ex.Message}", ex);
                Debug.WriteLine("Failed to load logs: " + ex.Message);
            }
        }

        public void GenerateTourReport()
        {
            try
            {
                string filePath = "TourReport.pdf";
                using (PdfWriter writer = new PdfWriter(filePath))
                {
                    using (PdfDocument pdf = new PdfDocument(writer))
                    {
                        Document document = new Document(pdf);
                        document.Add(new Paragraph("Tour Report").SetTextAlignment(TextAlignment.CENTER).SetFontSize(20));

                        foreach (var tour in Tours)
                        {
                            document.Add(new Paragraph($"Tour Name: {tour.Name}"));
                            document.Add(new Paragraph($"Description: {tour.Description}"));
                            document.Add(new Paragraph($"From: {tour.From} To: {tour.To}"));
                            document.Add(new Paragraph($"Route Type: {tour.RouteType}"));
                            document.Add(new Paragraph($"Distance: {tour.Distance} km"));
                            document.Add(new Paragraph($"Estimated Time: {tour.EstimatedTime}"));
                            document.Add(new Paragraph(new Text("\n")));
                        }

                        document.Close();
                    }
                }
                MessageBox.Show("PDF report generated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                log.Info("PDF report generated successfully.");
            }
            catch (Exception ex)
            {
                log.Error($"Error generating PDF report: {ex.Message}", ex);
                MessageBox.Show($"Error generating PDF report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

     public void GenerateLogReport()
{
    try
    {
        string logFilePath = "log-file.txt";
        string pdfFilePath = "LogReport.pdf";

        // Check if log file exists
        if (!File.Exists(logFilePath))
        {
            MessageBox.Show("Log file does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        // Check if log file is empty
        if (new FileInfo(logFilePath).Length == 0)
        {
            MessageBox.Show("Log file is empty. No content to generate a report.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        using (FileStream fs = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        using (StreamReader reader = new StreamReader(fs))
        {
            using (PdfWriter writer = new PdfWriter(pdfFilePath))
            {
                using (PdfDocument pdf = new PdfDocument(writer))
                {
                    Document document = new Document(pdf);
                    document.Add(new Paragraph("Log Report").SetFontSize(20).SetTextAlignment(TextAlignment.CENTER));

                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Sanitize each line to avoid invalid characters
                        string sanitizedLine = SanitizeLogLine(line);
                        document.Add(new Paragraph(sanitizedLine));
                    }

                    document.Close();
                }
            }
        }

        MessageBox.Show("Log report generated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        log.Info("Log report generated successfully.");
    }
    catch (IOException ex)
    {
        log.Error($"File access error: {ex.Message}", ex);
        MessageBox.Show($"File access error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
    catch (Exception ex)
    {
        log.Error($"Error generating log report: {ex.Message}", ex);
        MessageBox.Show($"Error generating log report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}

private string SanitizeLogLine(string line)
{
    // Replace any invalid or unsupported characters that might cause issues in PDF generation
    return line.Replace("\0", "").Replace("\r", "").Replace("\n", "").Replace("\t", " ");
}



private void ExecuteSearch()
{
    try
    {
        if (!string.IsNullOrWhiteSpace(SearchQuery))
        {
            var searchResults = _repository.SearchTours(SearchQuery);
            Tours.Clear();
            foreach (var tour in searchResults)
            {
                Tours.Add(tour);
            }

            // Wenn ein Suchergebnis existiert, wähle das erste aus
            if (Tours.Any())
            {
                SelectedTour = Tours.First();
            }

            log.Info("Search executed successfully.");
        }
        else {
            // Wenn Suchfeld leer lade alle Touren
            LoadTours();
            log.Info("All tours loaded successfully.");
        }
    }
    catch (Exception ex)
    {
        log.Error($"Search execution failed: {ex.Message}", ex);
        MessageBox.Show($"Search execution failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}




        public static class PolylineDecoder
        {
            public static List<(double Latitude, double Longitude)> DecodePolyline(string encodedPolyline)
            {
                var coordinates = new List<(double Latitude, double Longitude)>();
                int index = 0;
                int len = encodedPolyline.Length;
                int lat = 0;
                int lng = 0;

                while (index < len)
                {
                    lat += DecodeNextValue(encodedPolyline, ref index);
                    lng += DecodeNextValue(encodedPolyline, ref index);

                    double latitude = lat / 1e5;
                    double longitude = lng / 1e5;
                    coordinates.Add((latitude, longitude));
                }

                return coordinates;
            }

            private static int DecodeNextValue(string encodedPolyline, ref int index)
            {
                int shift = 0;
                int result = 0;
                int byteValue;

                do
                {
                    byteValue = encodedPolyline[index++] - 63;
                    result |= (byteValue & 0x1f) << shift;
                    shift += 5;
                } while (byteValue >= 0x20);

                // Decode to signed value
                if ((result & 1) != 0)
                {
                    result = ~(result >> 1);
                }
                else
                {
                    result >>= 1;
                }

                return result;
            }
        }
    }
}
