using FluentValidation;
using NtripCore.Manager.Client.Models.Dialogs;

namespace NtripCore.Manager.Client.Validation.Models.Dialogs
{
    public class NtripSettingsDialogModelValidator : AbstractValidator<NtripSettingsDialogModel>
    {
        public NtripSettingsDialogModelValidator()
        {
            RuleFor(i => i.Host)
                .NotEmpty()
                .When(i => i.NtripServiceEnabled)
                .WithMessage(i => $"Host is required.");

            RuleFor(i => i.Port)
                .NotNull()
                .When(i => i.NtripServiceEnabled)
                .WithMessage(i => $"Port is required.");

            RuleFor(i => i.Username)
                .NotEmpty()
                .When(i => i.NtripServiceEnabled)
                .WithMessage(i => $"Username is required.");

            RuleFor(i => i.Password)
                .NotEmpty()
                .When(i => i.NtripServiceEnabled)
                .WithMessage(i => $"Password is required.");

            RuleFor(i => i.Mountpoint)
                .NotEmpty()
                .When(i => i.NtripServiceEnabled)
                .WithMessage(i => $"Mountpoint is required.");
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<NtripSettingsDialogModel>.CreateWithOptions((NtripSettingsDialogModel)model, x => x.IncludeProperties(propertyName)));
            
            if (result.IsValid)
                return Array.Empty<string>();

            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}
