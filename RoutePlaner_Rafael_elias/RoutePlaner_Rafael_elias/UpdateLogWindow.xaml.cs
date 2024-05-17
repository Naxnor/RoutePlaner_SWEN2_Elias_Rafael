using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RoutePlaner_Rafael_elias.Models;
using RoutePlaner_Rafael_elias.ViewModels;

namespace RoutePlaner_Rafael_elias
{
    
    public partial class UpdateLogWindow : Window
    {
        public UpdateLogWindow(Log existingLog)
        {
            InitializeComponent();
            DataContext = new UpdateLogViewModel(existingLog);
        }
    }
}
