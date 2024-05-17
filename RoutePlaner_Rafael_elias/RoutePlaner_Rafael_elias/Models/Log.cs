namespace RoutePlaner_Rafael_elias.Models
{
    public class Log
    {
        public int Id { get; set; }
        public Tour Tour { get; set; }
        public DateTime Date { get; set; }
		public string Comment { get; set; }
		public decimal Distance { get; set; }
        public decimal Duration { get; set; }
		public int Rating { get; set;}
	
		public decimal Steps { get; set; }
        public string Weather { get; set; }
        public decimal Difficulty { get; set; }
        public int TourId { get; set; }
        public decimal TotalTime { get; set; }
    }
}



