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
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RoutePlaner_Rafael_elias.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly TourRepository _repository;
        private readonly RouteService _routeService;

        private Tour _selectedTour;
        private Log _selectedLog;
        private WebBrowser _mapBrowser;
        private ObservableCollection<Log> _dataGridLogList;
        private string _selectedTourDescription;
        private string _imagePath = @"F:\GIT\SWEN\RoutePlaner_Rafael_elias\RoutePlaner_Rafael_elias\Data\Images\image.png";

        public ICommand FetchRouteCommand { get; }
        public ICommand ShowMapCommand { get; }
        public ICommand OpenAddTourWindowCommand { get; private set; }
        public ICommand OpenUpdateTourWindowCommand { get; private set; }
        public ICommand DeleteTourCommand { get; private set; }
        public ICommand AddLogCommand { get; private set; }
        public ICommand UpdateLogCommand { get; private set; }
        public ICommand DeleteLogCommand { get; private set; }

        public MainViewModel()
        {
            _repository = new TourRepository();
            _routeService = new RouteService();

            FetchRouteCommand = new AsyncRelayCommand(FetchRouteData);
            ShowMapCommand = new RelayCommand(ShowMap);

            Tours = new ObservableCollection<Tour>();
            DataGridLogList = new ObservableCollection<Log>();

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
                    MessageBox.Show("Please select a tour first.");
                    return;
                }

                var routeData = await _routeService.GetDirectionsAsync(
                    SelectedTour.StartLatitude,
                    SelectedTour.StartLongitude,
                    SelectedTour.EndLatitude,
                    SelectedTour.EndLongitude
                );

                SelectedTour.Distance = routeData.Distance;
                SelectedTour.EstimatedTime = TimeSpan.FromSeconds(routeData.Duration);
                SelectedTour.EncodedRoute = routeData.EncodedPolyline;

                Debug.WriteLine($"Encoded Route: {SelectedTour.EncodedRoute}");

                _repository.UpdateTour(SelectedTour);
                ShowMap(); // Attempt to display the map
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching route data: {ex.Message}");
                MessageBox.Show($"Error fetching route data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ShowMap()
        {
            if (MapBrowser == null || string.IsNullOrEmpty(SelectedTour?.EncodedRoute))
            {
                MessageBox.Show("MapBrowser control is not set or route is not available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error navigating to URL: {ex.Message}");
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
                        _repository.DeleteTour(SelectedTour);
                        Tours.Remove(SelectedTour);
                        SelectedTour = null;
                        MessageBox.Show("Tour deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error deleting tour: {ex.Message}");
                        MessageBox.Show($"Error deleting tour: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
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
                }
                catch (Exception ex)
                {
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
            }
            catch (Exception ex)
            {
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
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load logs: " + ex.Message);
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
