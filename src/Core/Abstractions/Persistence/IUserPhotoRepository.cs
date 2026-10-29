using ClassManager.Core.Domain.Documents;
using ClassManager.Core.Domain.Users;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IUserPhotoRepository
{
    Task<UserPhoto?> FindByUserAsync(long userId, CancellationToken cancellationToken);

    Task<UserPhoto?> FindForUpdateAsync(long photoId, CancellationToken cancellationToken);

    Task<IReadOnlyList<UserPhoto>> ListAwaitingGuardianAsync(IReadOnlyCollection<long> userIds, CancellationToken cancellationToken);

    Task<IReadOnlyList<OwnedPhoto>> ListVisibleOfUsersAsync(IReadOnlyCollection<long> userIds, CancellationToken cancellationToken);

    Task<IReadOnlyList<OwnedPhoto>> ListVisibleOfStudentsAsync(IReadOnlyCollection<long> studentIds, CancellationToken cancellationToken);

    Task<IReadOnlyList<OwnedPhoto>> ListVisibleOfInstructorsAsync(IReadOnlyCollection<long> instructorIds, CancellationToken cancellationToken);

    Task<IReadOnlyList<Document>> DeleteEveryPhotoOfAccountAsync(Guid accountId, CancellationToken cancellationToken);

    void Add(UserPhoto photo);

    void Remove(UserPhoto photo);
}
