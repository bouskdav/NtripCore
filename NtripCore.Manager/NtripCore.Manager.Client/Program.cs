using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using NtripCore.Manager.Client.Services.BaseStation;
using NtripCore.Manager.Client.Services.Weather;
using NtripCore.Manager.Shared.Interfaces.Services.Api;

namespace NtripCore.Manager.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services.AddMudServices();

            // Preconfigure an HttpClient for web API calls
            builder.Services.AddSingleton(new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            // Register the client-side api services
            builder.Services.AddSingleton<IWeatherService, ClientWeatherService>();
            builder.Services.AddSingleton<IBaseStationService, ClientBaseStationService>();

            await builder.Build().RunAsync();
        }
    }
}
