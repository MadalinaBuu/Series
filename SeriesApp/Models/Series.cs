using System.ComponentModel.DataAnnotations;

namespace SeriesApp.Models
{
    public class Series
    {
        [Key]
        public int ID { get; set; }
        public int User_ID { get; set; }
        public string Name { get; set; }
        public string Source { get; set; }
        public int Seen { get; set; }
        public int Public { get; set; }
    }
    public class PartialSeries
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Source { get; set; }
    }
}