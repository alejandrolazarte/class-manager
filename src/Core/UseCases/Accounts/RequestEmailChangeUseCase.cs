using System.Net.Mail;
using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Notifications.Email;

namespace ClassManager.Core.UseCases.Accounts;

public sealed record RequestEmailChangeCommand(string? NewEmail, string? CurrentPassword) : ICommand;

public sealed record RequestEmailChangeResponse(string NewEmail);

public sealed class RequestEmailChangeUseCase(
    ICurrentUser currentUser,
    IIdentityService identityService,
    IEmailSender emailSender,
    IWebAppLinks webAppLinks)
    : IUseCase<RequestEmailChangeCommand, RequestEmailChangeResponse>
{
    public const string EmailSubject = "Confirmá tu nuevo email";
    public const int EmailMaxLength = 254;

    private const string EmailFormatMessage = "Email must be a valid address of at most 254 characters.";
    private const string CurrentPasswordRequiredMessage = "Write your current password.";

    public static EmailContent EmailContentFor(string confirmEmailChangeLink) =>
        new(
            "Tu cuenta",
            EmailSubject,
            "Pediste usar este email para entrar a la app. Tocá el botón para confirmarlo.",
            "Si no lo pediste, ignorá este mail: tu cuenta sigue igual.")
        {
            Action = new EmailAction("Confirmar email", confirmEmailChangeLink, ShowsLinkFallback: true),
            Note = new EmailNote("El link sirve una sola vez y vence en 24 horas."),
        };

    public async Task<Result<RequestEmailChangeResponse>> ExecuteAsync(RequestEmailChangeCommand command, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
        {
            return Result.Unauthorized<RequestEmailChangeResponse>(MemberErrorCodes.NoAccessMessage, MemberErrorCodes.NoAccess);
        }

        var newEmail = command.NewEmail?.Trim() ?? string.Empty;
        if (!IsValidEmail(newEmail))
        {
            return Result.Validation<RequestEmailChangeResponse>(EmailFormatMessage, fieldName: nameof(RequestEmailChangeCommand.NewEmail));
        }

        if (string.IsNullOrEmpty(command.CurrentPassword))
        {
            return Result.Validation<RequestEmailChangeResponse>(
                CurrentPasswordRequiredMessage, fieldName: nameof(RequestEmailChangeCommand.CurrentPassword));
        }

        var request = await identityService.RequestEmailChangeAsync(userId, command.CurrentPassword, newEmail, cancellationToken);
        if (request.IsFailure)
        {
            return request.Error!;
        }

        var content = EmailContentFor(webAppLinks.ConfirmEmailChange(request.Value!.Token));
        await emailSender.SendAsync(new EmailMessage(newEmail, EmailSubject, content), cancellationToken);

        return new RequestEmailChangeResponse(newEmail);
    }

    private static bool IsValidEmail(string email) =>
        email.Length is > 0 and <= EmailMaxLength
        && MailAddress.TryCreate(email, out var mailAddress)
        && string.Equals(mailAddress.Address, email, StringComparison.OrdinalIgnoreCase);
}
