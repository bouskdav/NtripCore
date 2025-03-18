using Ghostware.GPS.NET.Models.Events;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace NtripCore.Manager.Hubs
{
    public class ApplicationHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendGpsData(GpsDataEventArgs gpsData)
        {
            await Clients.All.SendAsync("ReceiveGpsData", "SYSTEM", gpsData);
        }
    }
}
