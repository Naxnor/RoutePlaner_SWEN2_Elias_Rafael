using System;
using System.Windows;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using RoutePlaner_Rafael_elias.Models;
using RoutePlaner_Rafael_elias.ViewModels;
using System.Linq;

namespace RoutePlaner_Rafael_elias
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Initialize GMap settings
            MapControl.MapProvider = GMapProviders.OpenStreetMap;
            MapControl.Position = new GMap.NET.PointLatLng(49.41461, 8.681495);
            MapControl.MinZoom = 2;
            MapControl.MaxZoom = 17;
            MapControl.Zoom = 5;
            MapControl.ShowCenter = false;

            // Verwende das MapControl direkt im Code-Behind
            var viewModel = (MainViewModel)DataContext;
            viewModel.RouteDataFetched += OnRouteDataFetched;
        }
        
        private void OnRouteDataFetched(object sender, EventArgs e)
        {
            if (sender is MainViewModel viewModel && viewModel.SelectedTour != null)
            {
                ShowMap(viewModel.SelectedTour);
            }
        }

        private void ShowMap(Tour selectedTour)
        {
            if (MapControl == null || string.IsNullOrEmpty(selectedTour?.EncodedRoute))
            {
                MessageBox.Show("Map control is not set or route is not available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var decodedCoordinates = MainViewModel.PolylineDecoder.DecodePolyline(selectedTour.EncodedRoute);
            if (decodedCoordinates.Count == 0)
            {
                MessageBox.Show("No route coordinates available to display.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MapControl.Position = new GMap.NET.PointLatLng(decodedCoordinates[0].Latitude, decodedCoordinates[0].Longitude);
            MapControl.Markers.Clear(); // Entferne alte Marker

            var route = new GMapRoute(decodedCoordinates.Select(c => new GMap.NET.PointLatLng(c.Latitude, c.Longitude)).ToList())
            {
                Shape = new System.Windows.Shapes.Path()
                {
                    Stroke = System.Windows.Media.Brushes.Blue,
                    StrokeThickness = 3
                }
            };

            MapControl.Markers.Add(new GMap.NET.WindowsPresentation.GMapMarker(new GMap.NET.PointLatLng(decodedCoordinates[0].Latitude, decodedCoordinates[0].Longitude)));
            MapControl.Markers.Add(new GMap.NET.WindowsPresentation.GMapMarker(new GMap.NET.PointLatLng(decodedCoordinates.Last().Latitude, decodedCoordinates.Last().Longitude)));
            MapControl.RegenerateShape(route);
            //MapControl.ZoomAndCenterRoutes(null);
        }
    }
}
