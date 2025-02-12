using Microsoft.AspNetCore.Mvc;
using WeatherForecast.Services;
using WeatherForecast.Data; 
using WeatherForecast.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherForecast.Controllers
{
    public class WeatherController : Controller
    {
        private readonly WeatherService _weatherService;
        private readonly ApplicationDbContext _context;

        public WeatherController(WeatherService weatherService, ApplicationDbContext context)
        {
            _weatherService = weatherService;
            _context = context;
        }

        public async Task<IActionResult> Index(string? city)
        {
            
            if (string.IsNullOrEmpty(city))
            {
                city = await _weatherService.GetLastUsedCityAsync();
                if (string.IsNullOrEmpty(city)) {
                    city = "Kyiv";
                }
            }

            var weather = await _weatherService.GetWeatherAsync(city);
            if (weather == null)
            {
                ViewBag.Error = "City not found or API error.";
                return View();
            }

            if (weather.WillRain)
            {
                if (!await _weatherService.IfCityAlerted(city))
                {
                    await _weatherService.SaveCityToAlert(city);
                    ViewBag.ShowWarning = true; 
                }
            }

            return View(weather);
        }
    }
}
