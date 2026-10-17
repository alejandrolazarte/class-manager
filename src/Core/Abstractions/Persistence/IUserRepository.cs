using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IUserRepository
{
    Task<Guid?> FindIdByAccountAsync(Guid businessId, Guid accountId, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<Guid, string>> ListFullNamesAsync(IReadOnlyCollection<Guid> userIds, CancellationToken cancellationToken);

    Task AddMissingInEveryBranchAsync(Guid organizationId, IReadOnlyList<UserAccount> accounts, DateTimeOffset createdAt, CancellationToken cancellationToken);

    Task ChangeEmailInEveryBusinessByAccountAsync(Guid accountId, string email, CancellationToken cancellationToken);
}
