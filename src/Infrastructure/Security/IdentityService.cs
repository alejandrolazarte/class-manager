using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Security.Accounts;

namespace ClassManager.Infrastructure.Security;

internal sealed class IdentityService(IUserAccountService userAccountService) : IIdentityService
{
    public Task<bool> IsEmailRegisteredAsync(string email, CancellationToken cancellationToken) =>
        userAccountService.IsEmailRegisteredAsync(email, cancellationToken);

    public async Task<Result<Guid>> CreateOwnerAsync(OwnerAccount account, CancellationToken cancellationToken)
    {
        var creation = await userAccountService.CreateAsync(
            new NewUserAccount(account.Email, account.Password, account.FullName),
            cancellationToken);

        return creation.Status switch
        {
            UserAccountCreationStatus.Created => creation.UserId,
            UserAccountCreationStatus.EmailTaken =>
                Result.Conflict<Guid>(AuthenticationErrorCodes.EmailTakenMessage, AuthenticationErrorCodes.EmailTaken),
            UserAccountCreationStatus.InvalidPassword =>
                Result.Validation<Guid>(creation.ErrorDescription!, fieldName: nameof(OwnerAccount.Password)),
            _ => Result.Validation<Guid>(creation.ErrorDescription!),
        };
    }

    public async Task<CredentialVerification> VerifyCredentialsAsync(string email, string password, CancellationToken cancellationToken)
    {
        var check = await userAccountService.VerifyCredentialsAsync(email, password, cancellationToken);

        return check.Status switch
        {
            CredentialCheckStatus.Verified => CredentialVerification.Verified(check.UserId, check.Email),
            CredentialCheckStatus.LockedOut => CredentialVerification.LockedOut,
            _ => CredentialVerification.InvalidCredentials,
        };
    }
}
