using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Users;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IUserRepository
{
    void Add(User user);

    void Remove(User user);

    Task<User?> FindByEmailForUpdateAsync(string email, long exceptUserId, CancellationToken cancellationToken);

    Task<PersonRoles> GetRolesAsync(long userId, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<long, long>> ListClientIdsByUserAsync(IReadOnlyCollection<long> userIds, CancellationToken cancellationToken);

    Task<IReadOnlySet<long>> ListOnTheTeamAsync(IReadOnlyCollection<long> userIds, CancellationToken cancellationToken);

    Task<long?> FindIdByAccountAsync(long businessId, Guid accountId, CancellationToken cancellationToken);

    Task<User?> FindByAccountForUpdateAsync(Guid accountId, CancellationToken cancellationToken);

    Task<bool> IsEmailTakenAsync(string email, long exceptUserId, CancellationToken cancellationToken);

    Task<IReadOnlySet<string>> ListEmailsAsync(CancellationToken cancellationToken);

    Task<bool> IsEmailTakenByAnotherPersonOfTheAccountsBusinessesAsync(Guid accountId, string email, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<long, string>> ListFullNamesAsync(IReadOnlyCollection<long> userIds, CancellationToken cancellationToken);

    Task AddMissingInEveryBranchAsync(long organizationId, IReadOnlyList<UserAccount> accounts, DateTimeOffset createdAt, CancellationToken cancellationToken);

    Task ChangeEmailInEveryBusinessByAccountAsync(Guid accountId, string email, CancellationToken cancellationToken);
}
