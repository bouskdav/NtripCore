using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using NtripCore.Manager.Client.Dialogs;
using NtripCore.Manager.Client.Models.Dialogs;
using NtripCore.Manager.Client.Models.Pages;
using NtripCore.Manager.Shared.Enums;
using NtripCore.Manager.Shared.Interfaces.Services.Api;
using NtripCore.Manager.Shared.Models.SystemState;
using NtripCore.Manager.Shared.Models.Weather;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NtripCore.Manager.Client.Pages
{
    public partial class Home// : IAsyncDisposable
    {
        [Inject] IBaseStationService BaseStationService { get; set; }
        [Inject] IDialogService DialogService { get; set; }
        [Inject] ISnackbar SnackBar { get; set; }
        [Inject] NavigationManager Navigation { get; set; }

        private System.Threading.Timer? _timer;
        private bool _editing = false;

        private HubConnection? hubConnection;

        public HomePageModel Model { get; set; } = new();

        public bool IsConnected => hubConnection?.State == HubConnectionState.Connected;

        protected override async Task OnInitializedAsync()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl(Navigation.ToAbsoluteUri("/applicationhub"))
                .Build();

            hubConnection.On<string, string>("ReceiveGpsData", (user, message) =>
            {
                var encodedMsg = $"{user}: {message}";
                //messages.Add(encodedMsg);
                Console.WriteLine(encodedMsg);

                InvokeAsync(StateHasChanged);
            });

            await hubConnection.StartAsync();

            var systemState = await BaseStationService.GetSystemState();

            Model.OperatingMode = systemState.OperatingMode;
            Model.IsGpsServiceRunning = systemState.IsGpsServiceRunning;

            _timer = new System.Threading.Timer(async (object? stateInfo) =>
            {
                var systemState = await BaseStationService.GetSystemState();

                Console.WriteLine(JsonSerializer.Serialize(systemState));

                await InvokeAsync(() =>
                {
                    // service states - update always
                    Model.IsGpsServiceRunning = systemState.IsGpsServiceRunning;

                    // operating mode - don't update when editing
                    if (!_editing)
                    {
                        Model.OperatingMode = systemState.OperatingMode;
                    }

                    StateHasChanged(); // must be invoked
                });
            },
            new System.Threading.AutoResetEvent(false), 4000, 5000); // fire every 2000 milliseconds
        }

        private async Task OperatingModeChanged(OperatingModeType operatingMode)
        {
            _editing = true;

            var oldOperatingMode = Model.OperatingMode;

            Model.OperatingMode = operatingMode;

            // for GPS, ask for NTRIP
            if (operatingMode == OperatingModeType.Gps)
            {
                // ask for NTRIP
                var result = await OpenNtripSettingsDialogAsync();

                // if not canceled
                if (result != null)
                {
                    Model.NtripServiceEnabled = result.NtripServiceEnabled;
                    Model.Host = result.Host;
                    Model.Port = result.Port;
                    Model.Username = result.Username;
                    Model.Password = result.Password;
                    Model.Mountpoint = result.Mountpoint;
                }
                else
                {
                    SnackBar.Add($"Result canceled.", Severity.Info);

                    Model.OperatingMode = oldOperatingMode;
                    _editing = false;

                    return;
                }
            }

            try
            {
                var stateReport = await BaseStationService.SetOperatingMode(Model);

                if (stateReport.OperatingMode != Model.OperatingMode)
                    throw new Exception($"Error switching modes.");
            }
            catch (Exception ex)
            {
                SnackBar.Add($"Error switching modes.", Severity.Info);

                Model.OperatingMode = oldOperatingMode;
            }

            _editing = false;
        }

        private async Task<NtripSettingsDialogModel> OpenNtripSettingsDialogAsync()
        {
            var parameters = new DialogParameters
            {
                {
                    nameof(NtripSettingsDialog.Model), new NtripSettingsDialogModel()
                    {
                        NtripServiceEnabled = Model.NtripServiceEnabled,
                        Host = Model.Host,
                        Port = Model.Port,
                        Username = Model.Username,
                        Password = Model.Password,
                        Mountpoint = Model.Mountpoint,
                    }
                },
            };

            var options = new DialogOptions { CloseOnEscapeKey = true };
            //var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, BackdropClick = false };

            var dialog = await DialogService.ShowAsync<NtripSettingsDialog>("Sample dialog", parameters, options);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                return (NtripSettingsDialogModel)result.Data;
            }

            return null;
        }

        //public async ValueTask DisposeAsync()
        //{
        //    if (hubConnection is not null)
        //    {
        //        await hubConnection.DisposeAsync();
        //    }
        //}
    }
}
