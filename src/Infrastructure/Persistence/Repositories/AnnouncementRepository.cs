namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class AnnouncementRepository(AppDbContext context) : IAnnouncementRepository
{
    public void Add(Announcement announcement) => context.Announcements.Add(announcement);

    public void Remove(Announcement announcement) => context.Announcements.Remove(announcement);

    public void AddView(AnnouncementView view) => context.AnnouncementViews.Add(view);

    public void AddReply(AnnouncementReply reply) => context.AnnouncementReplies.Add(reply);

    public Task<Announcement?> FindForUpdateAsync(Guid announcementId, CancellationToken cancellationToken) =>
        context.Announcements.FirstOrDefaultAsync(announcement => announcement.Id == announcementId, cancellationToken);

    public Task<Announcement?> FindDeletedForUpdateAsync(Guid announcementId, CancellationToken cancellationToken) =>
        context.Announcements
            .IgnoreQueryFilters([SoftDeleteModelBuilderExtensions.SoftDeleteQueryFilter])
            .FirstOrDefaultAsync(announcement => announcement.Id == announcementId && announcement.IsDeleted, cancellationToken);

    public Task<Announcement?> FindPinnedForUpdateAsync(CancellationToken cancellationToken) =>
        context.Announcements.FirstOrDefaultAsync(announcement => announcement.IsPinned, cancellationToken);

    public Task<AnnouncementReply?> FindReplyForUpdateAsync(Guid announcementId, Guid clientId, CancellationToken cancellationToken) =>
        context.AnnouncementReplies.FirstOrDefaultAsync(
            reply => reply.AnnouncementId == announcementId && reply.ClientId == clientId, cancellationToken);

    public async Task<IReadOnlyList<Announcement>> ListRecentOrPinnedAsync(DateTimeOffset publishedSince, CancellationToken cancellationToken) =>
        await context.Announcements.AsNoTracking()
            .Where(announcement => announcement.PublishedAt >= publishedSince || announcement.IsPinned)
            .OrderByDescending(announcement => announcement.PublishedAt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<AnnouncementView>> ListViewsAsync(IReadOnlyCollection<Guid> announcementIds, CancellationToken cancellationToken) =>
        await context.AnnouncementViews.AsNoTracking()
            .Where(view => announcementIds.Contains(view.AnnouncementId))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<AnnouncementReply>> ListRepliesAsync(IReadOnlyCollection<Guid> announcementIds, CancellationToken cancellationToken) =>
        await context.AnnouncementReplies.AsNoTracking()
            .Where(reply => announcementIds.Contains(reply.AnnouncementId))
            .ToListAsync(cancellationToken);
}
