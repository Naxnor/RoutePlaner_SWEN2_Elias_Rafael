using System.Windows;
using System.Diagnostics;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System.Threading.Tasks;
using System.Windows.Input;
using RoutePlaner_Rafael_elias.Helpers;

namespace RoutePlaner_Rafael_elias
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeMap(); // Karte beim Start initialisieren
        }

        // Methode zur Initialisierung der Karte
        public void InitializeMap()
        {
            GMapControl map = new GMapControl();
            map.MapProvider = GMapProviders.OpenStreetMap;
            map.Position = new GMap.NET.PointLatLng(48.8589507, 2.2770208); // Beispielkoordinaten (Paris, Frankreich)
            map.MinZoom = 2;
            map.MaxZoom = 17;
            map.Zoom = 5;

            this.Content = map; // Setzt die Karte als Inhalt des Fensters
        }

        // Methode zur Anzeige der Route auf der Karte
        public async void DisplayRouteOnMap(string startLocation, string endLocation)
        {
            var routeService = new RouteService();
            var jsonHelper = new JsonHelper();

            try
            {
                string routeData = await routeService.GetRouteAsync(startLocation, endLocation);
                double distance = jsonHelper.ExtractDistance(routeData);
                double duration = jsonHelper.ExtractDuration(routeData);

                MessageBox.Show($"Distance: {distance / 1000} km, Duration: {duration / 60} min");

                InitializeMap(); // Optional, falls du die Karte neu initialisieren möchtest
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // Beispiel für die Aufrufmethode beim Klicken auf eine Schaltfläche (erfordert XAML-Änderung)
        private void OnShowRouteClick(object sender, RoutedEventArgs e)
        {
            // Beispielkoordinaten, diese sollten durch tatsächliche Eingabewerte ersetzt werden
            string start = "8.681495,49.41461"; // Startposition (Längengrad, Breitengrad)
            string end = "8.687872,49.420318";   // Endposition (Längengrad, Breitengrad)

            DisplayRouteOnMap(start, end);
        }
    }
}
