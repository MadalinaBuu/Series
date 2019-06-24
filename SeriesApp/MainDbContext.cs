using SeriesApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SeriesApp
{
    public class MainDbContext : DbContext
    {
        public MainDbContext()
            : base("name=connStrSeries")
        {
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Series> Series { get; set; }
        public DbSet<Episode> Episodes { get; set; }
    }
}