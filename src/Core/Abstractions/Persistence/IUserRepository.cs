using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Users;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IUserRepository
{
    void Add(User user);

    void Remove(User user);

    Task<User?> FindByEmailForUpdateAsync(string email, Guid exceptUserId, CancellationToken cancellationToken);

    Task<PersonRoles> GetRolesAsync(Guid userId, CancellationToken cancellationToken);

    Task<Guid?> FindIdByAccountAsync(Guid businessId, Guid accountId, CancellationToken cancellationToken);

    Task<User?> FindByAccountForUpdateAsync(Guid accountId, CancellationToken cancellationToken);

    Task<bool> IsEmailTakenAsync(string email, Guid exceptUserId, CancellationToken cancellationToken);

    Task<IReadOnlySet<string>> ListEmailsAsync(CancellationToken cancellationToken);

    Task<bool> IsEmailTakenByAnotherPersonOfTheAccountsBusinessesAsync(Guid accountId, string email, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<Guid, string>> ListFullNamesAsync(IReadOnlyCollection<Guid> userIds, CancellationToken cancellationToken);

    Task AddMissingInEveryBranchAsync(Guid organizationId, IReadOnlyList<UserAccount> accounts, DateTimeOffset createdAt, CancellationToken cancellationToken);

    Task ChangeEmailInEveryBusinessByAccountAsync(Guid accountId, string email, CancellationToken cancellationToken);
}
