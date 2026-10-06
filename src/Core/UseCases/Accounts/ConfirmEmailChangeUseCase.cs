using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Notifications.Email;

namespace ClassManager.Core.UseCases.Accounts;

public sealed record ConfirmEmailChangeCommand(string? Token) : ICommand;

public sealed record ConfirmEmailChangeResponse(string NewEmail);

public sealed class ConfirmEmailChangeUseCase(
    IIdentityService identityService,
    IClientRepository clientRepository,
    IInstructorRepository instructorRepository,
    IEmailSender emailSender)
    : IUseCase<ConfirmEmailChangeCommand, ConfirmEmailChangeResponse>
{
    public const string NoticeSubject = "Cambiaste el email de tu cuenta";

    public static EmailContent NoticeContentFor(string newEmail) =>
        new(
            "Tu cuenta",
            NoticeSubject,
            $"Desde ahora entrás a la app con {newEmail}.",
            "Si no fuiste vos, avisá enseguida al lugar donde tomás clases o trabajás.");

    public async Task<Result<ConfirmEmailChangeResponse>> ExecuteAsync(ConfirmEmailChangeCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Token))
        {
            return Result.Validation<ConfirmEmailChangeResponse>(
                AuthenticationErrorCodes.InvalidEmailChangeTokenMessage, AuthenticationErrorCodes.InvalidEmailChangeToken);
        }

        var change = await identityService.ConfirmEmailChangeAsync(command.Token, cancellationToken);
        if (change.IsFailure)
        {
            return change.Error!;
        }

        var (userId, previousEmail, newEmail) = change.Value!;
        await clientRepository.ChangeEmailInEveryBusinessByAccountUserAsync(userId, newEmail, cancellationToken);
        await instructorRepository.ChangeEmailInEveryBusinessByMemberUserAsync(userId, newEmail, cancellationToken);
        await emailSender.SendAsync(new EmailMessage(previousEmail, NoticeSubject, NoticeContentFor(newEmail)), cancellationToken);

        return new ConfirmEmailChangeResponse(newEmail);
    }
}
