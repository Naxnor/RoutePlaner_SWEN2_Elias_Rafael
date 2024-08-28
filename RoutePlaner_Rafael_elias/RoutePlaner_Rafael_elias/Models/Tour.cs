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
            public double StartLatitude { get; set; }
            public double StartLongitude { get; set; }
            public double EndLatitude { get; set; }
            public double EndLongitude { get; set; }
            public string EncodedRoute { get; set; }
            
            public List<Log>? Logs { get; set; } = new List<Log>(); // Initialized to avoid null
            public List<Log>? LogList { get; set; } = new List<Log>(); // Initialized to avoid null
    }
}
