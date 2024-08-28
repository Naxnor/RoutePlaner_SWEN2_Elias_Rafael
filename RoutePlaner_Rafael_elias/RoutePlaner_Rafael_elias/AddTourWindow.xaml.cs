using System.Windows;
using System.Windows.Input;
using GMap.NET;
using GMap.NET.WindowsPresentation;
using RoutePlaner_Rafael_elias.ViewModels;

namespace RoutePlaner_Rafael_elias
{
    public partial class AddTourWindow : Window
    {
        private bool _isSettingStartPoint = true; // Flag to toggle between setting start and end points

        public AddTourWindow()
        {
            InitializeComponent();
            DataContext = new AddTourViewModel();

            // Configure the map
            MapControl.MapProvider = GMap.NET.MapProviders.GMapProviders.OpenStreetMap;
            MapControl.Position = new PointLatLng(47.36865, 8.53918); // Default position
            MapControl.MinZoom = 2;
            MapControl.MaxZoom = 17;
            MapControl.Zoom = 5;
            MapControl.ShowCenter = false;
            MapControl.MouseLeftButtonDown += MapControl_MouseLeftButtonDown;
        }

        private void MapControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var viewModel = (AddTourViewModel)DataContext;

            // Get the clicked point's coordinates
            var point = e.GetPosition(MapControl);
            var latLng = MapControl.FromLocalToLatLng((int)point.X, (int)point.Y);

            if (_isSettingStartPoint)
            {
                // Set start coordinates
                viewModel.StartLatitude = latLng.Lat;
                viewModel.StartLongitude = latLng.Lng;
                _isSettingStartPoint = false; // Toggle to setting end point
            }
            else
            {
                // Set end coordinates
                viewModel.EndLatitude = latLng.Lat;
                viewModel.EndLongitude = latLng.Lng;
                _isSettingStartPoint = true; // Reset to setting start point
            }
        }
    }
}