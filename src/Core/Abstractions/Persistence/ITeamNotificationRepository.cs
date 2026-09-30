using ClassManager.Core.Domain.Notifications;

namespace ClassManager.Core.Abstractions.Persistence;

public interface ITeamNotificationRepository
{
    Task<IReadOnlyList<TeamNotification>> ListLatestAsync(Guid recipientUserId, int limit, CancellationToken cancellationToken);

    Task<int> CountUnseenAsync(Guid recipientUserId, CancellationToken cancellationToken);

    Task<IReadOnlyList<TeamNotification>> ListUnseenForUpdateAsync(Guid recipientUserId, CancellationToken cancellationToken);
}
