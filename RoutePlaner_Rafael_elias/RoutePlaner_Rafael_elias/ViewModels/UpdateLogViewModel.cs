using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using RoutePlaner_Rafael_elias.Models;
using RoutePlaner_Rafael_elias.Repository;
using System;
using System.Windows;
using System.Windows.Input;

namespace RoutePlaner_Rafael_elias.ViewModels
{
    public class UpdateLogViewModel : ObservableObject
    {
        private readonly TourRepository _repository;
        private Log _log;

        public Log Log
        {
            get => _log;
            set => SetProperty(ref _log, value);
        }

        public ICommand ExecuteUpdateLog { get; }

        public UpdateLogViewModel(Log existingLog)
        {
            _repository = new TourRepository();
            Log = existingLog;
            ExecuteUpdateLog = new RelayCommand(UpdateLog);
        }

        private void UpdateLog()
        {
            try
            {
                _repository.UpdateLog(Log);
                MessageBox.Show("Log updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                CloseWindow();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating log: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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