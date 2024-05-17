using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using RoutePlaner_Rafael_elias.Models;
using RoutePlaner_Rafael_elias.Repository;
using System;
using System.Windows;
using System.Windows.Input;

namespace RoutePlaner_Rafael_elias.ViewModels
{
    public class AddTourViewModel : ObservableObject
    {
        private readonly TourRepository _repository;
        private Tour _tour;

        public Tour Tour
        {
            get => _tour;
            set => SetProperty(ref _tour, value);
        }

        public ICommand AddTourCommand { get; }

        public AddTourViewModel()
        {
            _tour = new Tour(); 
            _repository = new TourRepository();
            AddTourCommand = new RelayCommand(AddTour);
        }

        public void AddTour()
        {
            try
            {
                _repository.AddTour(_tour);
                
                MessageBox.Show("Tour added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                
            }
            catch (Exception ex)
            {
              
                Console.WriteLine($"Error adding tour: {ex.Message}");
                MessageBox.Show($"Error adding tour: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}