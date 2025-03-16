using NtripCore.Manager.Shared.Constants;
using NtripCore.Manager.Shared.Interfaces.Services.Api;
using NtripCore.Manager.Shared.Models.Weather;
using System.Net.Http.Json;

namespace NtripCore.Manager.Client.Services.Weather
{
    public class ClientWeatherService(HttpClient httpClient) : IWeatherService
    {
        public Task<WeatherForecast[]> GetWeather() =>
            httpClient.GetFromJsonAsync<WeatherForecast[]>(RouteConstants.Weather.GetWeather)!;
    }
}
