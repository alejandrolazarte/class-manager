namespace ClassManager.Core.Abstractions.Persistence;

public interface IUserRepository
{
    Task ChangeEmailInEveryBusinessByAccountAsync(Guid accountId, string email, CancellationToken cancellationToken);
}
