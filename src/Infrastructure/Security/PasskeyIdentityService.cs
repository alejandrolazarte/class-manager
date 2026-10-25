using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.UseCases.Passkeys;
using ClassManager.Security.Accounts;
using ClassManager.Security.Passkeys;

namespace ClassManager.Infrastructure.Security;

internal sealed class PasskeyIdentityService(IPasskeyService passkeyService) : IPasskeyIdentityService
{
    public async Task<Result<StartedPasskeyCeremony>> BeginRegistrationAsync(Guid userId, string currentPassword, CancellationToken cancellationToken)
    {
        var start = await passkeyService.BeginRegistrationAsync(userId, currentPassword, cancellationToken);

        return start.Status switch
        {
            PasskeyRegistrationStartStatus.Started => Ceremony(start.Ceremony!),
            PasskeyRegistrationStartStatus.InvalidPassword => Result.Validation<StartedPasskeyCeremony>(
                AuthenticationErrorCodes.InvalidCurrentPasswordMessage,
                AuthenticationErrorCodes.InvalidCurrentPassword,
                nameof(BeginPasskeyRegistrationCommand.CurrentPassword)),
            _ => Result.Unauthorized<StartedPasskeyCeremony>(
                AuthenticationErrorCodes.InvalidRefreshTokenMessage, AuthenticationErrorCodes.InvalidRefreshToken),
        };
    }

    public async Task<Result<AddedPasskey>> FinishRegistrationAsync(
        Guid userId,
        string ceremonyId,
        string credentialJson,
        string name,
        CancellationToken cancellationToken)
    {
        var registration = await passkeyService.FinishRegistrationAsync(userId, ceremonyId, credentialJson, name, cancellationToken);

        return registration.Status switch
        {
            PasskeyRegistrationStatus.Registered => new AddedPasskey(Passkey(registration.Passkey!), registration.Email!),
            PasskeyRegistrationStatus.InvalidCeremony => Result.Validation<AddedPasskey>(
                AuthenticationErrorCodes.InvalidPasskeyCeremonyMessage,
                AuthenticationErrorCodes.InvalidPasskeyCeremony,
                nameof(FinishPasskeyRegistrationCommand.CeremonyId)),
            _ => Result.Validation<AddedPasskey>(
                AuthenticationErrorCodes.InvalidPasskeyMessage,
                AuthenticationErrorCodes.InvalidPasskey,
                nameof(FinishPasskeyRegistrationCommand.Credential)),
        };
    }

    public async Task<StartedPasskeyCeremony> BeginSignInAsync(CancellationToken cancellationToken) =>
        Ceremony(await passkeyService.BeginSignInAsync(cancellationToken));

    public async Task<CredentialVerification> VerifyPasskeyAsync(string ceremonyId, string credentialJson, CancellationToken cancellationToken)
    {
        var check = await passkeyService.SignInAsync(ceremonyId, credentialJson, cancellationToken);

        return check.Status switch
        {
            CredentialCheckStatus.Verified => CredentialVerification.Verified(check.UserId, check.Email),
            CredentialCheckStatus.LockedOut => CredentialVerification.LockedOut,
            _ => CredentialVerification.InvalidCredentials,
        };
    }

    public async Task<IReadOnlyList<AccountPasskey>> ListAsync(Guid userId, CancellationToken cancellationToken) =>
        [.. (await passkeyService.ListAsync(userId, cancellationToken)).Select(Passkey)];

    public Task<bool> RemoveAsync(Guid userId, string passkeyId, CancellationToken cancellationToken) =>
        passkeyService.RemoveAsync(userId, passkeyId, cancellationToken);

    private static StartedPasskeyCeremony Ceremony(PasskeyCeremonyStart start) => new(start.CeremonyId, start.OptionsJson);

    private static AccountPasskey Passkey(UserPasskeySummary passkey) => new(passkey.Id, passkey.Name, passkey.CreatedAt);
}
