using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Authentication;

public sealed record RequestPasswordResetCommand(string? Email);

public sealed record RequestPasswordResetResponse;

public sealed class RequestPasswordResetUseCase(
    IIdentityService identityService,
    IEmailSender emailSender,
    IWebAppLinks webAppLinks)
    : IUseCase<RequestPasswordResetCommand, RequestPasswordResetResponse>
{
    public const string EmailSubject = "Elegí una contraseña nueva";

    public static string EmailBody(string resetPasswordLink) =>
        "Hola,\n\n" +
        "Pediste cambiar tu contraseña. Abrí este link para elegir una nueva:\n\n" +
        $"{resetPasswordLink}\n\n" +
        "El link sirve una sola vez y vence en 24 horas. Si no lo pediste, ignorá este email: tu contraseña no cambia.";

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
            var body = EmailBody(webAppLinks.ResetPassword(token));
            await emailSender.SendAsync(new EmailMessage(email, EmailSubject, body), cancellationToken);
        }

        return new RequestPasswordResetResponse();
    }
}
