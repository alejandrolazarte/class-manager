using ClassManager.Core.Domain.Announcements;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IAnnouncementRepository
{
    void Add(Announcement announcement);

    void Remove(Announcement announcement);

    void AddView(AnnouncementView view);

    void AddReply(AnnouncementReply reply);

    Task<Announcement?> FindForUpdateAsync(long announcementId, CancellationToken cancellationToken);

    Task<Announcement?> FindDeletedForUpdateAsync(long announcementId, CancellationToken cancellationToken);

    Task<Announcement?> FindPinnedForUpdateAsync(CancellationToken cancellationToken);

    Task<AnnouncementReply?> FindReplyForUpdateAsync(long announcementId, long clientId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Announcement>> ListRecentOrPinnedAsync(DateTimeOffset publishedSince, CancellationToken cancellationToken);

    Task<IReadOnlyList<AnnouncementView>> ListViewsAsync(IReadOnlyCollection<long> announcementIds, CancellationToken cancellationToken);

    Task<IReadOnlyList<AnnouncementReply>> ListRepliesAsync(IReadOnlyCollection<long> announcementIds, CancellationToken cancellationToken);
}
