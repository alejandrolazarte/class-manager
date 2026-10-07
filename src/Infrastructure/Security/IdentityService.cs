using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.UseCases.Authentication;
using ClassManager.Security.Accounts;

namespace ClassManager.Infrastructure.Security;

internal sealed class IdentityService(
    IUserAccountService userAccountService,
    IPasswordResetService passwordResetService,
    IEmailChangeService emailChangeService)
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

    public Task<bool> ChangeFullNameAsync(Guid userId, string fullName, CancellationToken cancellationToken) =>
        userAccountService.ChangeFullNameAsync(userId, fullName, cancellationToken);

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

    public async Task<Result<EmailChangeRequested>> RequestEmailChangeAsync(
        Guid userId,
        string currentPassword,
        string newEmail,
        CancellationToken cancellationToken)
    {
        var request = await emailChangeService.RequestAsync(userId, currentPassword, newEmail, cancellationToken);

        return request.Status switch
        {
            EmailChangeRequestStatus.Requested => new EmailChangeRequested(request.Token!, request.CurrentEmail!),
            EmailChangeRequestStatus.InvalidPassword => Result.Validation<EmailChangeRequested>(
                AuthenticationErrorCodes.InvalidCurrentPasswordMessage,
                AuthenticationErrorCodes.InvalidCurrentPassword,
                nameof(ClassManager.Core.UseCases.Accounts.RequestEmailChangeCommand.CurrentPassword)),
            EmailChangeRequestStatus.EmailTaken => Result.Conflict<EmailChangeRequested>(
                AuthenticationErrorCodes.EmailTakenMessage, AuthenticationErrorCodes.EmailTaken),
            EmailChangeRequestStatus.SameEmail => Result.Validation<EmailChangeRequested>(
                AuthenticationErrorCodes.SameEmailMessage,
                AuthenticationErrorCodes.SameEmail,
                nameof(ClassManager.Core.UseCases.Accounts.RequestEmailChangeCommand.NewEmail)),
            _ => Result.Unauthorized<EmailChangeRequested>(
                AuthenticationErrorCodes.InvalidRefreshTokenMessage, AuthenticationErrorCodes.InvalidRefreshToken),
        };
    }

    public async Task<Result<ConfirmedEmailChange>> ConfirmEmailChangeAsync(string token, CancellationToken cancellationToken)
    {
        var change = await emailChangeService.ConfirmAsync(token, cancellationToken);

        return change.Status switch
        {
            EmailChangeStatus.Changed => new ConfirmedEmailChange(change.UserId, change.PreviousEmail!, change.NewEmail!),
            EmailChangeStatus.EmailTaken => Result.Conflict<ConfirmedEmailChange>(
                AuthenticationErrorCodes.EmailTakenMessage, AuthenticationErrorCodes.EmailTaken),
            _ => Result.Validation<ConfirmedEmailChange>(
                AuthenticationErrorCodes.InvalidEmailChangeTokenMessage, AuthenticationErrorCodes.InvalidEmailChangeToken),
        };
    }
}
