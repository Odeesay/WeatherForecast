using Microsoft.EntityFrameworkCore;
using WeatherForecast.Data;
using WeatherForecast.Services;

var builder = WebApplication.CreateBuilder(args);

// Додаємо підключення до бази даних MSSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<WeatherService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Weather}/{action=Index}/{city?}");

app.Run();
