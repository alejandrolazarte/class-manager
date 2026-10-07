namespace ClassManager.Security.Accounts;

public interface IUserAccountService
{
    Task<bool> IsEmailRegisteredAsync(string email, CancellationToken cancellationToken);

    Task<Guid?> FindUserIdByEmailAsync(string email, CancellationToken cancellationToken);

    Task<IReadOnlyList<UserAccountSummary>> ListAsync(IReadOnlyCollection<Guid> userIds, CancellationToken cancellationToken);

    Task<UserAccountCreation> CreateAsync(NewUserAccount account, CancellationToken cancellationToken);

    Task<bool> ChangeFullNameAsync(Guid userId, string fullName, CancellationToken cancellationToken);

    Task<CredentialCheck> VerifyCredentialsAsync(string email, string password, CancellationToken cancellationToken);
}
