using ClassManager.Tenancy.AspNetCore.Persistence;

namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository(AppDbContext context) : IUserRepository
{
    public Task ChangeEmailInEveryBusinessByAccountAsync(Guid accountId, string email, CancellationToken cancellationToken) =>
        context.Users
            .IgnoreTenantFilter()
            .Where(user => user.AccountId == accountId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(user => user.Email, email), cancellationToken);
}
