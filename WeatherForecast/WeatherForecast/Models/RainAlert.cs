using System;
using System.ComponentModel.DataAnnotations;

namespace WeatherForecast.Models
{
    public class RainAlert
    {
        [Key]
        public int Id { get; set; }
        public string City { get; set; } = null!;
        public DateTime AlertDate { get; set; }

        public RainAlert() { }
    }

}
