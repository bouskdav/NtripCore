using NtripCore.Manager.Shared.Models.Weather;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtripCore.Manager.Shared.Interfaces.Services.Api
{
    public interface IWeatherService
    {
        Task<WeatherForecast[]> GetWeather();
    }
}
