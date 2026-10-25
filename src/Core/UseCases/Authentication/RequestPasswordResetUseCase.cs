using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.Emails;

namespace ClassManager.Core.UseCases.Authentication;

public sealed record RequestPasswordResetCommand([property: AuditPersonal] string? Email) : ICommand;

public sealed record RequestPasswordResetResponse;

public sealed class RequestPasswordResetUseCase(
    IIdentityService identityService,
    IEmailSender emailSender,
    IWebAppLinks webAppLinks)
    : IUseCase<RequestPasswordResetCommand, RequestPasswordResetResponse>
{
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
            var emailTexts = await RecipientEmailTexts.OfEmailAsync(identityService, email, cancellationToken);
            var passwordReset = emailTexts.PasswordReset(webAppLinks.ResetPassword(token, email));
            await emailSender.SendAsync(new EmailMessage(email, passwordReset.Subject, passwordReset.Content), cancellationToken);
        }

        return new RequestPasswordResetResponse();
    }
}
