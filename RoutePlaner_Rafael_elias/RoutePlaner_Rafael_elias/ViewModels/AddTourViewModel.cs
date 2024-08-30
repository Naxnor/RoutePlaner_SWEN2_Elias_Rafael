using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using RoutePlaner_Rafael_elias.Models;
using RoutePlaner_Rafael_elias.Repository;

namespace RoutePlaner_Rafael_elias.ViewModels
{
    public class AddTourViewModel : ObservableObject
    {
        private readonly TourRepository _repository;
        private double? _startLatitude;
        private double? _startLongitude;
        private double? _endLatitude;
        private double? _endLongitude;
        private string _name;
        private string _from;
        private string _to;
        private string _description;
        private string _routeType;

        public AddTourViewModel()
        {
            _repository = new TourRepository();
            AddTourCommand = new RelayCommand(AddTour);
            RouteTypes = new ObservableCollection<string> { "bike", "walking", "driving-car" }; // Initialize RouteTypes
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string From
        {
            get => _from;
            set => SetProperty(ref _from, value);
        }

        public string To
        {
            get => _to;
            set => SetProperty(ref _to, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string RouteType
        {
            get => _routeType;
            set => SetProperty(ref _routeType, value);
        }

        public double? StartLatitude
        {
            get => _startLatitude;
            set => SetProperty(ref _startLatitude, value);
        }

        public double? StartLongitude
        {
            get => _startLongitude;
            set => SetProperty(ref _startLongitude, value);
        }

        public double? EndLatitude
        {
            get => _endLatitude;
            set => SetProperty(ref _endLatitude, value);
        }

        public double? EndLongitude
        {
            get => _endLongitude;
            set => SetProperty(ref _endLongitude, value);
        }

        public ObservableCollection<string> RouteTypes { get; } // Collection of route types for ComboBox

        public ICommand AddTourCommand { get; }

        private void AddTour()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                MessageBox.Show("Please enter a name for the tour.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var tour = new Tour
            {
                Name = Name,
                From = From,
                To = To,
                Description = Description,
                RouteType = RouteType,
                StartLatitude = StartLatitude ?? 0,
                StartLongitude = StartLongitude ?? 0,
                EndLatitude = EndLatitude ?? 0,
                EndLongitude = EndLongitude ?? 0
            };

            try
            {
                _repository.AddTour(tour);
                MessageBox.Show("Tour added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                CloseCurrentWindow(); // Fenster schließen nach dem Hinzufügen der Tour
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while adding the tour: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseCurrentWindow()
        {
            Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(window => window.DataContext == this)
                ?.Close();
        }
    }
}
