using System;
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
            set => SetProperty(ref _tour, value);
        }

        public ICommand UpdateTourCommand { get; }

        public UpdateTourViewModel(Tour existingTour)
        {
            _repository = new TourRepository();
            Tour = existingTour;
            UpdateTourCommand = new RelayCommand(UpdateTour);
        }

        private void UpdateTour()
        {
            try
            {
                _repository.UpdateTour(Tour);
                MessageBox.Show("Tour updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                CloseWindow();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating tour: {ex.Message}");
             
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
