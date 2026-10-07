using ClassManager.Core.Common;
using ClassManager.Core.Domain.Accounts;

namespace ClassManager.Core.Abstractions.Security;

public interface IIdentityService
{
    Task<bool> IsEmailRegisteredAsync(string email, CancellationToken cancellationToken);

    Task<Guid?> FindUserIdByEmailAsync(string email, CancellationToken cancellationToken);

    Task<IReadOnlyList<UserAccount>> ListAccountsAsync(IReadOnlyCollection<Guid> userIds, CancellationToken cancellationToken);

    Task<Result<Guid>> CreateOwnerAsync(OwnerAccount account, CancellationToken cancellationToken);

    Task<bool> ChangeFullNameAsync(Guid userId, string fullName, CancellationToken cancellationToken);

    Task<CredentialVerification> VerifyCredentialsAsync(string email, string password, CancellationToken cancellationToken);

    Task<string?> CreatePasswordResetTokenAsync(string email, CancellationToken cancellationToken);

    Task<Result> ResetPasswordAsync(string token, string newPassword, CancellationToken cancellationToken);

    Task<Result<EmailChangeRequested>> RequestEmailChangeAsync(
        Guid userId,
        string currentPassword,
        string newEmail,
        CancellationToken cancellationToken);

    Task<Result<ConfirmedEmailChange>> ConfirmEmailChangeAsync(string token, CancellationToken cancellationToken);
}
