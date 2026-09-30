using ClassManager.Core.Domain.Announcements;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IAnnouncementRepository
{
    void Add(Announcement announcement);

    void Remove(Announcement announcement);

    Task<Announcement?> FindForUpdateAsync(Guid announcementId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Announcement>> ListPublishedSinceAsync(DateTimeOffset since, CancellationToken cancellationToken);
}
