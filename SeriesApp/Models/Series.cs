using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeriesApp.Models
{
    [Table("Series")]
    public class Series
    {
        [Key]
        public int ID { get; set; }
        public int User_ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Source { get; set; }
        public int Seen { get; set; }
        public bool Public { get; set; }
    }
}