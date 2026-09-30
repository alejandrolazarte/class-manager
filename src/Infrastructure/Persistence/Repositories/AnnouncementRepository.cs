namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class AnnouncementRepository(AppDbContext context) : IAnnouncementRepository
{
    public void Add(Announcement announcement) => context.Announcements.Add(announcement);

    public void Remove(Announcement announcement) => context.Announcements.Remove(announcement);

    public Task<Announcement?> FindForUpdateAsync(Guid announcementId, CancellationToken cancellationToken) =>
        context.Announcements.FirstOrDefaultAsync(announcement => announcement.Id == announcementId, cancellationToken);

    public async Task<IReadOnlyList<Announcement>> ListPublishedSinceAsync(DateTimeOffset since, CancellationToken cancellationToken) =>
        await context.Announcements.AsNoTracking()
            .Where(announcement => announcement.PublishedAt >= since)
            .OrderByDescending(announcement => announcement.PublishedAt)
            .ToListAsync(cancellationToken);
}
