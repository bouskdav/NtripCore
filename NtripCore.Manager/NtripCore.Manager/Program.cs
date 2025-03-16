using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using NtripCore.Manager.Client.Pages;
using NtripCore.Manager.Components;
using NtripCore.Manager.Data.Context;
using NtripCore.Manager.Hubs;
using NtripCore.Manager.Services.BaseStation;
using NtripCore.Manager.Services.System.Linux;
using NtripCore.Manager.Services.System.Windows;
using NtripCore.Manager.Services.Weather;
using NtripCore.Manager.Shared.Constants;
using NtripCore.Manager.Shared.Interfaces.Services.Api;
using NtripCore.Manager.Shared.Interfaces.Services.System;
using NtripCore.Manager.Shared.Models.SystemState;
using System.Runtime.InteropServices;

namespace NtripCore.Manager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSignalR();

            // Add MudBlazor services
            builder.Services.AddMudServices();

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveWebAssemblyComponents();

            // Register the server-side api services
            builder.Services.AddSingleton<IWeatherService, ServerWeatherService>();
            builder.Services.AddSingleton<IBaseStationService, ServerBaseStationService>();

            // system services
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                builder.Services.AddSingleton<IGpsServiceManager, LinuxGpsdManager>();
            else
                builder.Services.AddSingleton<IGpsServiceManager, WindowsGpsServiceManager>(); // dummy

            // database
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(connectionString));

            // register signalr hubs
            builder.Services.AddSingleton<ApplicationHub>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.MapStaticAssets();
            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

            // weather routes
            app.MapGet(RouteConstants.Weather.GetWeather, (IWeatherService weatherService) => weatherService.GetWeather());

            // base station routes
            app.MapGet(RouteConstants.BaseStation.GetSystemState, (IBaseStationService baseStationService) => baseStationService.GetSystemState());
            app.MapPost(RouteConstants.BaseStation.SetOperatingMode, (SetOperatingModeRequest request, IBaseStationService baseStationService) => baseStationService.SetOperatingMode(request));
            app.MapPost(RouteConstants.BaseStation.SetGpsServiceState, (SetGpsServiceRequest request, IBaseStationService baseStationService) => baseStationService.SetGpsServiceState(request));

            app.MapHub<ApplicationHub>("/applicationhub");

            app.Run();
        }
    }
}
