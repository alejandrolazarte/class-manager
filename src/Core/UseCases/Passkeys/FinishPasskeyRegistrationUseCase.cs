using System.Text.Json;
using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Emails;

namespace ClassManager.Core.UseCases.Passkeys;

public sealed record FinishPasskeyRegistrationCommand(
    [property: AuditSecret] string? CeremonyId,
    [property: AuditSecret] JsonElement? Credential,
    [property: AuditPersonal] string? Name)
    : ICommand;

public sealed class FinishPasskeyRegistrationUseCase(
    ICurrentUser currentUser,
    IPasskeyIdentityService passkeyIdentityService,
    IIdentityService identityService,
    IEmailSender emailSender)
    : IUseCase<FinishPasskeyRegistrationCommand, PasskeyResponse>
{
    public const int NameMaxLength = 60;

    private const string NameFormatMessage = "Name the passkey with at most 60 characters.";

    public async Task<Result<PasskeyResponse>> ExecuteAsync(FinishPasskeyRegistrationCommand command, CancellationToken cancellationToken)
    {
        if (currentUser.AccountId is not { } accountId)
        {
            return Result.Unauthorized<PasskeyResponse>(MemberErrorCodes.NoAccessMessage, MemberErrorCodes.NoAccess);
        }

        if (string.IsNullOrEmpty(command.CeremonyId))
        {
            return PasskeyCommands.MissingCeremony<PasskeyResponse>(nameof(FinishPasskeyRegistrationCommand.CeremonyId));
        }

        if (!PasskeyCommands.HasCredential(command.Credential))
        {
            return PasskeyCommands.MissingCredential<PasskeyResponse>(nameof(FinishPasskeyRegistrationCommand.Credential));
        }

        var name = command.Name?.Trim() ?? string.Empty;
        if (name.Length is 0 or > NameMaxLength)
        {
            return Result.Validation<PasskeyResponse>(NameFormatMessage, fieldName: nameof(FinishPasskeyRegistrationCommand.Name));
        }

        var registration = await passkeyIdentityService.FinishRegistrationAsync(
            accountId, command.CeremonyId, command.Credential!.Value.GetRawText(), name, cancellationToken);
        if (registration.IsFailure)
        {
            return registration.Error!;
        }

        var addedPasskey = registration.Value!;
        var emailTexts = await RecipientEmailTexts.OfUserAsync(identityService, accountId, cancellationToken);
        var notice = emailTexts.PasskeyAddedNotice(addedPasskey.Passkey.Name);
        await emailSender.SendAsync(new EmailMessage(addedPasskey.Email, notice.Subject, notice.Content), cancellationToken);

        return PasskeyResponse.From(addedPasskey.Passkey);
    }
}
