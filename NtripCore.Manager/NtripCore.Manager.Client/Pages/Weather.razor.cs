using Microsoft.AspNetCore.Components;
using NtripCore.Manager.Shared.Interfaces.Services.Api;
using NtripCore.Manager.Shared.Models.Weather;

namespace NtripCore.Manager.Client.Pages
{
    public partial class Weather
    {
        [Inject] IWeatherService WeatherService { get; set; }

        private WeatherForecast[]? forecasts;

        protected override async Task OnInitializedAsync()
        {
            // Simulate asynchronous loading to demonstrate a loading indicator
            await Task.Delay(500);

            //var startDate = DateOnly.FromDateTime(DateTime.Now);
            //var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
            //forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            //{
            //    Date = startDate.AddDays(index),
            //    TemperatureC = Random.Shared.Next(-20, 55),
            //    Summary = summaries[Random.Shared.Next(summaries.Length)]
            //}).ToArray();

            forecasts = await WeatherService.GetWeather();
        }
    }
}
