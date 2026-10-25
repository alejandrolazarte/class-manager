using ClassManager.Core.Common;

namespace ClassManager.Core.Abstractions.Security;

public interface IPasskeyIdentityService
{
    Task<Result<StartedPasskeyCeremony>> BeginRegistrationAsync(Guid userId, string currentPassword, CancellationToken cancellationToken);

    Task<Result<AddedPasskey>> FinishRegistrationAsync(
        Guid userId,
        string ceremonyId,
        string credentialJson,
        string name,
        CancellationToken cancellationToken);

    Task<StartedPasskeyCeremony> BeginSignInAsync(CancellationToken cancellationToken);

    Task<CredentialVerification> VerifyPasskeyAsync(string ceremonyId, string credentialJson, CancellationToken cancellationToken);

    Task<IReadOnlyList<AccountPasskey>> ListAsync(Guid userId, CancellationToken cancellationToken);

    Task<bool> RemoveAsync(Guid userId, string passkeyId, CancellationToken cancellationToken);
}
