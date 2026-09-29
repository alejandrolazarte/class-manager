using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Security.Accounts;

internal sealed class UserAccountService(UserManager<ApplicationUser> userManager) : IUserAccountService
{
    private const string DuplicateEmailErrorCode = nameof(IdentityErrorDescriber.DuplicateEmail);
    private const string DuplicateUserNameErrorCode = nameof(IdentityErrorDescriber.DuplicateUserName);
    private const string PasswordErrorCodePrefix = "Password";

    public async Task<bool> IsEmailRegisteredAsync(string email, CancellationToken cancellationToken) =>
        await userManager.FindByEmailAsync(email) is not null;

    public async Task<Guid?> FindUserIdByEmailAsync(string email, CancellationToken cancellationToken) =>
        (await userManager.FindByEmailAsync(email))?.Id;

    public async Task<IReadOnlyList<UserAccountSummary>> ListAsync(IReadOnlyCollection<Guid> userIds, CancellationToken cancellationToken) =>
        userIds.Count == 0
            ? []
            : await userManager.Users
                .AsNoTracking()
                .Where(user => userIds.Contains(user.Id))
                .Select(user => new UserAccountSummary(user.Id, user.Email ?? string.Empty, user.FullName))
                .ToListAsync(cancellationToken);

    public async Task<UserAccountCreation> CreateAsync(NewUserAccount account, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            Id = Guid.CreateVersion7(),
            UserName = account.Email,
            Email = account.Email,
            FullName = account.FullName,
        };

        var creation = await userManager.CreateAsync(user, account.Password);
        if (creation.Succeeded)
        {
            return UserAccountCreation.Created(user.Id);
        }

        if (creation.Errors.Any(error => error.Code is DuplicateEmailErrorCode or DuplicateUserNameErrorCode))
        {
            return UserAccountCreation.EmailTaken;
        }

        var firstError = creation.Errors.First();

        return firstError.Code.StartsWith(PasswordErrorCodePrefix, StringComparison.Ordinal)
            ? UserAccountCreation.InvalidPassword(firstError.Description)
            : UserAccountCreation.Invalid(firstError.Description);
    }

    public async Task<CredentialCheck> VerifyCredentialsAsync(string email, string password, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return CredentialCheck.InvalidCredentials;
        }

        if (await userManager.IsLockedOutAsync(user))
        {
            return CredentialCheck.LockedOut;
        }

        if (!await userManager.CheckPasswordAsync(user, password))
        {
            await userManager.AccessFailedAsync(user);
            return CredentialCheck.InvalidCredentials;
        }

        await userManager.ResetAccessFailedCountAsync(user);
        return CredentialCheck.Verified(user.Id, user.Email ?? email);
    }
}
