using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Notifications.Email;

namespace ClassManager.Core.UseCases.Authentication;

public sealed record RequestPasswordResetCommand(string? Email) : ICommand;

public sealed record RequestPasswordResetResponse;

public sealed class RequestPasswordResetUseCase(
    IIdentityService identityService,
    IEmailSender emailSender,
    IWebAppLinks webAppLinks)
    : IUseCase<RequestPasswordResetCommand, RequestPasswordResetResponse>
{
    public const string EmailSubject = "Elegí una contraseña nueva";

    public static EmailContent EmailContentFor(string resetPasswordLink) =>
        new(
            "Tu cuenta",
            EmailSubject,
            "Pediste cambiar tu contraseña. Tocá el botón para elegir una nueva.",
            "Si no lo pediste, ignorá este mail: tu contraseña no cambia.")
        {
            Action = new EmailAction("Elegir contraseña", resetPasswordLink, ShowsLinkFallback: true),
            Note = new EmailNote("El link sirve una sola vez y vence en 24 horas."),
        };

    public async Task<Result<RequestPasswordResetResponse>> ExecuteAsync(
        RequestPasswordResetCommand command,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Email))
        {
            return new RequestPasswordResetResponse();
        }

        var email = command.Email.Trim();
        var token = await identityService.CreatePasswordResetTokenAsync(email, cancellationToken);
        if (token is not null)
        {
            var content = EmailContentFor(webAppLinks.ResetPassword(token));
            await emailSender.SendAsync(new EmailMessage(email, EmailSubject, content), cancellationToken);
        }

        return new RequestPasswordResetResponse();
    }
}
