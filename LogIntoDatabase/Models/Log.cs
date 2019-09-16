using System;
using System.ComponentModel.DataAnnotations;

namespace LogIntoDatabase.Models
{
    public class Log
    {
        [Key]
        public int Id { get; set; }
        public string Before { get; set; }
        public string After { get; set; }
        public string Url { get; set; }
        public string Type { get; set; }
        public DateTime DateUpdate { get; set; }
    }
}
