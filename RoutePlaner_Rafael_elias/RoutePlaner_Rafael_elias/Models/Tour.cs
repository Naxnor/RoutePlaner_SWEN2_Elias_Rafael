using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutePlaner_Rafael_elias.Models
{
   
    public class Tour
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(2000)]
        public string Description { get; set; }

        [StringLength(50)]
        public string From { get; set; }

        [StringLength(50)]
        public string To { get; set; }

        [StringLength(50)]
        public string RouteType { get; set; }

        public double StartLatitude { get; set; }
        public double StartLongitude { get; set; }
        public double EndLatitude { get; set; }
        public double EndLongitude { get; set; }
        public string EncodedRoute { get; set; }
        public double Distance { get; set; }
        public TimeSpan EstimatedTime { get; set; }

        public ICollection<Log> Logs { get; set; }
    }
}
