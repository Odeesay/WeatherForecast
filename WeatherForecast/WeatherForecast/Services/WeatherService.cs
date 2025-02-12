using System.Text.Json;
using WeatherForecast.Models;
using WeatherForecast.Data;
using Microsoft.EntityFrameworkCore;

namespace WeatherForecast.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbContext;
        private readonly string? _baseUrl;
        private readonly string? _apiKey;

        public WeatherService(HttpClient httpClient, IConfiguration configuration, ApplicationDbContext dbContext)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _dbContext = dbContext;
            _baseUrl = configuration["WeatherApi:BaseUrl"];
            _apiKey = configuration["WeatherApi:ApiKey"];
        }

        public async Task<WeatherData> GetWeatherAsync(string city)
        {
            string url = $"{_baseUrl}?q={city}&units=metric&appid={_apiKey}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var data = await response.Content.ReadFromJsonAsync<JsonElement>();
            var weather = new WeatherData
            {
                City = city,
                TemperatureMin = data.GetProperty("main").GetProperty("temp_min").GetDouble(),
                TemperatureMax = data.GetProperty("main").GetProperty("temp_max").GetDouble(),
                Description = data.GetProperty("weather")[0].GetProperty("description").GetString(),
                WillRain = (data.GetProperty("weather")[0].GetProperty("main").GetString()
                ?.Contains("rain", StringComparison.OrdinalIgnoreCase) ?? false)
                || (data.TryGetProperty("rain", out var rainProp) &&
                    rainProp.TryGetProperty("1h", out var rainAmount) &&
                    rainAmount.GetDouble() > 0)
            };

            await SaveLastUsedCityAsync(city);
            return weather;
        }

        private async Task SaveLastUsedCityAsync(string city)
        {
            var lastUsedCity = await _dbContext.UserLastCity.FirstOrDefaultAsync();
            if (lastUsedCity == null)
            {
                lastUsedCity = new UserLastCity { City = city, LastUpdated = DateTime.UtcNow };
                _dbContext.UserLastCity.Add(lastUsedCity);
            }
            else
            {
                lastUsedCity.City = city;
                lastUsedCity.LastUpdated = DateTime.UtcNow;
                _dbContext.UserLastCity.Update(lastUsedCity);
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task SaveCityToAlert(string city) {
            RainAlert rainAlert = new RainAlert { City = city, AlertDate = DateTime.Now };
            _dbContext.RainAlerts.Add(rainAlert);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> IfCityAlerted(string city) { 
            return await _dbContext.RainAlerts.AnyAsync(a => a.City == city && a.AlertDate.Date == DateTime.Today);
        }

        public async Task<string?> GetLastUsedCityAsync()
        {
            var lastUsedCity = await _dbContext.UserLastCity.FirstOrDefaultAsync();
            return lastUsedCity?.City;
        }
    }
}
