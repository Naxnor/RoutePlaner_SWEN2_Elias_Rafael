using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using RoutePlaner_Rafael_elias.Models;
using RoutePlaner_Rafael_elias.Repository;
using System;
using System.Windows;
using System.Windows.Input;

namespace RoutePlaner_Rafael_elias.ViewModels
{
    public class AddLogViewModel : ObservableObject
    {
        private readonly TourRepository _repository;
        private Log _logAddUtility;

        public Log LogAddUtility
        {
            get => _logAddUtility;
            set => SetProperty(ref _logAddUtility, value);
        }

        public ICommand ExecuteUpdateLog { get; }

        public AddLogViewModel()
        {
            _repository = new TourRepository();
            LogAddUtility = new Log();
            ExecuteUpdateLog = new RelayCommand(AddLog);
        }

        private void AddLog()
        {
            try
            {
                _repository.AddLog(LogAddUtility); 
                MessageBox.Show("Tour log updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

         
                CloseCurrentWindow();

               
                WeakReferenceMessenger.Default.Send(new LogUpdatedMessage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating tour log: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

    public class LogUpdatedMessage { }
}