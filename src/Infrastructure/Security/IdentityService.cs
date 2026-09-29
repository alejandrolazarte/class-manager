using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.UseCases.Authentication;
using ClassManager.Security.Accounts;

namespace ClassManager.Infrastructure.Security;

internal sealed class IdentityService(IUserAccountService userAccountService, IPasswordResetService passwordResetService)
    : IIdentityService
{
    public Task<bool> IsEmailRegisteredAsync(string email, CancellationToken cancellationToken) =>
        userAccountService.IsEmailRegisteredAsync(email, cancellationToken);

    public Task<Guid?> FindUserIdByEmailAsync(string email, CancellationToken cancellationToken) =>
        userAccountService.FindUserIdByEmailAsync(email, cancellationToken);

    public async Task<IReadOnlyList<UserAccount>> ListAccountsAsync(IReadOnlyCollection<Guid> userIds, CancellationToken cancellationToken) =>
        [
            .. (await userAccountService.ListAsync(userIds, cancellationToken))
                .Select(account => new UserAccount(account.UserId, account.Email, account.FullName)),
        ];

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

    public Task<string?> CreatePasswordResetTokenAsync(string email, CancellationToken cancellationToken) =>
        passwordResetService.CreateTokenAsync(email, cancellationToken);

    public async Task<Result> ResetPasswordAsync(string token, string newPassword, CancellationToken cancellationToken)
    {
        var reset = await passwordResetService.ResetAsync(token, newPassword, cancellationToken);

        return reset.Status switch
        {
            PasswordResetStatus.Reset => Result.Success(),
            PasswordResetStatus.InvalidPassword =>
                Result.Validation(reset.ErrorDescription!, fieldName: nameof(ResetPasswordCommand.NewPassword)),
            _ => Result.Validation(
                AuthenticationErrorCodes.InvalidPasswordResetTokenMessage,
                AuthenticationErrorCodes.InvalidPasswordResetToken),
        };
    }
}
