using System.ComponentModel.DataAnnotations;

namespace WeatherForecast.Models
{
    
    public class UserLastCity
    {
        [Key]
        public int UserId { get; set; } 
        public string? City { get; set; }  
        public DateTime LastUpdated { get; set; }

        public UserLastCity() { }
    }

    

}


