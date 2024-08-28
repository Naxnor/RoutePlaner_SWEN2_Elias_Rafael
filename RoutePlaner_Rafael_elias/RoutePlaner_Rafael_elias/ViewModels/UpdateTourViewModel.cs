using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using RoutePlaner_Rafael_elias.Models;
using RoutePlaner_Rafael_elias.Repository;

namespace RoutePlaner_Rafael_elias.ViewModels
{
    public class UpdateTourViewModel : ObservableObject
    {
        private readonly TourRepository _repository;
        private Tour _tour;

        public Tour Tour
        {
            get => _tour;
            set
            {
                SetProperty(ref _tour, value);
                SelectedRouteType = _tour.RouteType;  // Ensure the selected route type is set
            }
        }

        public ObservableCollection<string> RouteTypes { get; }

        private string _selectedRouteType;
        public string SelectedRouteType
        {
            get => _selectedRouteType;
            set => SetProperty(ref _selectedRouteType, value);
        }

        public ICommand UpdateTourCommand { get; }

        public UpdateTourViewModel(Tour existingTour)
        {
            _repository = new TourRepository();
            RouteTypes = new ObservableCollection<string> { "bike", "walking", "driving-car" }; // Initialize RouteTypes
            Tour = existingTour;
            UpdateTourCommand = new RelayCommand(UpdateTour);
        }

        private void UpdateTour()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Tour.Name))
                {
                    MessageBox.Show("Please enter a name for the tour.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Tour.RouteType = SelectedRouteType;  // Ensure the selected route type is set before updating
                _repository.UpdateTour(Tour);
                MessageBox.Show("Tour updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                CloseWindow();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating tour: {ex.Message}");
                MessageBox.Show($"Error updating tour: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                    break;
                }
            }
        }
    }
}
