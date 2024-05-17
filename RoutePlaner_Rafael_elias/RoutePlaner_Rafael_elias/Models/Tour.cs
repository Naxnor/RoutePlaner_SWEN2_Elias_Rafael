using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutePlaner_Rafael_elias.Models
{
   
        public class Tour
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string From { get; set; }
            public string To { get; set; }
            public string? RouteType { get; set; }
            public double Distance { get; set; }
            public TimeSpan EstimatedTime { get; set; }
        
        public string MapImagePath { get; set; }
    
        public virtual ICollection<TourLog> Logs { get; set; } 
        public ObservableCollection<Log> LogList { get; set; }
    }
}
