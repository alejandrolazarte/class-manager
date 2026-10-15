using ClassManager.Core.Domain.Announcements;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IAnnouncementRepository
{
    void Add(Announcement announcement);

    void Remove(Announcement announcement);

    void AddView(AnnouncementView view);

    void AddReply(AnnouncementReply reply);

    Task<Announcement?> FindForUpdateAsync(Guid announcementId, CancellationToken cancellationToken);

    Task<Announcement?> FindDeletedForUpdateAsync(Guid announcementId, CancellationToken cancellationToken);

    Task<Announcement?> FindPinnedForUpdateAsync(CancellationToken cancellationToken);

    Task<AnnouncementReply?> FindReplyForUpdateAsync(Guid announcementId, Guid clientId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Announcement>> ListRecentOrPinnedAsync(DateTimeOffset publishedSince, CancellationToken cancellationToken);

    Task<IReadOnlyList<AnnouncementView>> ListViewsAsync(IReadOnlyCollection<Guid> announcementIds, CancellationToken cancellationToken);

    Task<IReadOnlyList<AnnouncementReply>> ListRepliesAsync(IReadOnlyCollection<Guid> announcementIds, CancellationToken cancellationToken);
}
