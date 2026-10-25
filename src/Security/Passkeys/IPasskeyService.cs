using ClassManager.Security.Accounts;

namespace ClassManager.Security.Passkeys;

/// <summary>
/// Passkey (WebAuthn) registration and sign-in over ASP.NET Core Identity's passkey handler, with the state between
/// the two requests of each ceremony kept in <see cref="PasskeyCeremony"/> instead of a cookie.
/// </summary>
public interface IPasskeyService
{
    Task<PasskeyRegistrationStart> BeginRegistrationAsync(Guid userId, string currentPassword, CancellationToken cancellationToken);

    Task<PasskeyRegistration> FinishRegistrationAsync(
        Guid userId,
        string ceremonyId,
        string credentialJson,
        string name,
        CancellationToken cancellationToken);

    Task<PasskeyCeremonyStart> BeginSignInAsync(CancellationToken cancellationToken);

    Task<CredentialCheck> SignInAsync(string ceremonyId, string credentialJson, CancellationToken cancellationToken);

    Task<IReadOnlyList<UserPasskeySummary>> ListAsync(Guid userId, CancellationToken cancellationToken);

    Task<bool> RemoveAsync(Guid userId, string passkeyId, CancellationToken cancellationToken);
}
