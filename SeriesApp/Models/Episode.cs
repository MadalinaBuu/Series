using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SeriesApp.Models
{
    public class Episode
    {
        [Key]
        public int ID { get; set; }
        public int Serial { get; set; }
        public int Season { get; set; }
        public int No { get; set; }
        public string Title { get; set; }
        public int Seen { get; set; }
    }
}