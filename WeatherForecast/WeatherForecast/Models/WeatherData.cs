namespace WeatherForecast.Models
{
    public class WeatherData
    {
        public string? City { get; set; }
        public double TemperatureMin { get; set; }
        public double TemperatureMax { get; set; }
        public string? Description { get; set; }
        public bool WillRain { get; set; }
    }
}
