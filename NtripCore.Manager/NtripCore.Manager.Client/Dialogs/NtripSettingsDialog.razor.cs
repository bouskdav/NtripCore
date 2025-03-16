using Microsoft.AspNetCore.Components;
using MudBlazor;
using NtripCore.Manager.Client.Models.Dialogs;
using NtripCore.Manager.Client.Validation.Models.Dialogs;

namespace NtripCore.Manager.Client.Dialogs
{
    public partial class NtripSettingsDialog
    {
        [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }

        [Parameter] public NtripSettingsDialogModel Model { get; set; }

        private MudForm form;

        private NtripSettingsDialogModelValidator validator = new NtripSettingsDialogModelValidator();

        private async Task Submit()
        {
            await form.Validate();

            if (form.IsValid)
            {
                MudDialog.Close(DialogResult.Ok(Model));
            }
        }

        private void Cancel() => MudDialog.Cancel();
    }
}
