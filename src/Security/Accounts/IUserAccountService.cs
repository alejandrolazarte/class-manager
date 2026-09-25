namespace ClassManager.Security.Accounts;

public interface IUserAccountService
{
    Task<bool> IsEmailRegisteredAsync(string email, CancellationToken cancellationToken);

    Task<UserAccountCreation> CreateAsync(NewUserAccount account, CancellationToken cancellationToken);

    Task<CredentialCheck> VerifyCredentialsAsync(string email, string password, CancellationToken cancellationToken);
}
