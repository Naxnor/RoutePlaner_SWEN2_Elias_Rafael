using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using RoutePlaner_Rafael_elias.Models;
using RoutePlaner_Rafael_elias.Repository;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RoutePlaner_Rafael_elias.Services;

namespace RoutePlaner_Rafael_elias.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly TourRepository _repository;
        private Tour _selectedTour;
        private Log _selectedLog;
        private RouteService _routeService;
        private WebBrowser _mapBrowser;
        private ObservableCollection<Log> _dataGridLogList;
        private string _selectedTourDescription;
        private string _imagePath = @"F:\GIT\SWEN\RoutePlaner_Rafael_elias\RoutePlaner_Rafael_elias\Data\Images\image.png";
        private string _routeData;

        public ICommand FetchRouteCommand { get; }
        public ICommand ShowMapCommand { get; }

        public MainViewModel()
        {
            _repository = new TourRepository();
            _routeService = new RouteService();

            FetchRouteCommand = new RelayCommand(async () => await FetchRouteData());
            ShowMapCommand = new RelayCommand(ShowMap);

            Tours = new ObservableCollection<Tour>();
            LoadTours();
            InitializeCommands();

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


        public async Task FetchRouteData()
        {
            try
            {
                if (SelectedTour == null)
                {
                    MessageBox.Show("Please select a tour first.");
                    return;
                }

                // Ensure we pass four separate coordinates
                var routeData = await _routeService.GetDirectionsAsync(
                    SelectedTour.StartLatitude, 
                    SelectedTour.StartLongitude, 
                    SelectedTour.EndLatitude, 
                    SelectedTour.EndLongitude
                );

                // Store fetched route data in the tour model
                SelectedTour.Distance = routeData.Distance;
                SelectedTour.EstimatedTime = TimeSpan.FromSeconds(routeData.Duration);
                SelectedTour.EncodedRoute = routeData.EncodedPolyline;

                // Save updated tour data to the database
                _repository.UpdateTour(SelectedTour);

                // Display the map with the fetched route data
                ShowMap();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching route data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ShowMap()
        {
            if (MapBrowser == null || string.IsNullOrEmpty(_routeData)) return;

            string htmlString = GenerateMapHtml(_routeData);
            MapBrowser.NavigateToString(htmlString);
        }

        private string GenerateMapHtml(string routeData)
        {
            return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <title>Map</title>
            <link rel='stylesheet' href='https://unpkg.com/leaflet/dist/leaflet.css' />
            <script src='https://unpkg.com/leaflet/dist/leaflet.js'></script>
        </head>
        <body>
            <div id='map' style='width: 100%; height: 400px;'></div>
            <script>
                var map = L.map('map').setView([49.41461, 8.681495], 13);
                L.tileLayer('https://{{s}}.tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png', {{
                    maxZoom: 19,
                }}).addTo(map);

                // Add route polyline using routeData
                var route = L.polyline({routeData}, {{color: 'blue'}}).addTo(map);

                // Add markers for start and end points
                L.marker([49.41461, 8.681495]).addTo(map);
                L.marker([49.420318, 8.687872]).addTo(map);
            </script>
        </body>
        </html>";
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

        public ICommand OpenAddTourWindowCommand { get; private set; }
        public ICommand OpenUpdateTourWindowCommand { get; private set; }
        public ICommand DeleteTourCommand { get; private set; }
        public ICommand AddLogCommand { get; private set; }
        public ICommand UpdateLogCommand { get; private set; }
        public ICommand DeleteLogCommand { get; private set; }

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
            LoadTours();
        }

        private void OpenUpdateTourWindow()
        {
            if (SelectedTour != null)
            {
                var window = new UpdateTourWindow(SelectedTour);
                window.ShowDialog();
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
        }

        private void OpenUpdateLogWindow()
        {
            if (SelectedLog != null)
            {
                var window = new UpdateLogWindow(SelectedLog);
                window.ShowDialog();
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
                    tour.RouteType = GetRouteTypeForTour(tour);
                    Tours.Add(tour);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load tours: " + ex.Message);
            }
        }

        private string GetRouteTypeForTour(Tour tour)
        {
            return _repository.GetRouteTypeForTour(tour.Id);
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
    }
}
