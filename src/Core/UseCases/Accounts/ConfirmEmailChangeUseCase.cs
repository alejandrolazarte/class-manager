using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.Emails;

namespace ClassManager.Core.UseCases.Accounts;

public sealed record ConfirmEmailChangeCommand([property: AuditSecret] string? Token) : ICommand;

public sealed record ConfirmEmailChangeResponse(string NewEmail);

public sealed class ConfirmEmailChangeUseCase(
    IIdentityService identityService,
    IUserRepository userRepository,
    IEmailSender emailSender)
    : IUseCase<ConfirmEmailChangeCommand, ConfirmEmailChangeResponse>
{
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

        var (accountId, previousEmail, newEmail) = change.Value!;
        await userRepository.ChangeEmailInEveryBusinessByAccountAsync(accountId, newEmail, cancellationToken);
        var emailTexts = await RecipientEmailTexts.OfUserAsync(identityService, accountId, cancellationToken);
        var notice = emailTexts.EmailChangedNotice(newEmail);
        await emailSender.SendAsync(new EmailMessage(previousEmail, notice.Subject, notice.Content), cancellationToken);

        return new ConfirmEmailChangeResponse(newEmail);
    }
}
