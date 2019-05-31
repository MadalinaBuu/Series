﻿using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SeriesApp.Models
{
    public class Users
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Name { get; set; }

        public string Country { get; set; }

    }
}