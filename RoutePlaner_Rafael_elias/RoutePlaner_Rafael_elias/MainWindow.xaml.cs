using System.Windows;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;

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
        }
    }
}