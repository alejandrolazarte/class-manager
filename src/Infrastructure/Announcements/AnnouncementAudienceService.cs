using ClassManager.Infrastructure.Persistence;

namespace ClassManager.Infrastructure.Announcements;

internal sealed class AnnouncementAudienceService(AppDbContext context, TimeProvider timeProvider) : IAnnouncementAudienceService
{
    public async Task<IReadOnlyDictionary<Guid, IReadOnlyList<AnnouncementRecipient>>> RecipientsOfAsync(
        IReadOnlyList<Announcement> announcements, CancellationToken cancellationToken)
    {
        if (announcements.Count == 0)
        {
            return new Dictionary<Guid, IReadOnlyList<AnnouncementRecipient>>();
        }

        var clientsWithApp = await (
            from client in context.Clients.AsNoTracking()
            where context.ClientAccounts.Any(account => account.ClientId == client.Id)
            orderby client.FullName
            select new AnnouncementRecipient(client.Id, client.FullName))
            .ToListAsync(cancellationToken);
        var classGroupIds = announcements.SelectMany(announcement => announcement.ClassGroupIds).Distinct().ToList();
        var clientIdsByClassGroup = await ClientIdsByClassGroupAsync(classGroupIds, cancellationToken);

        return announcements.ToDictionary(
            announcement => announcement.Id,
            announcement => announcement.IsForEveryone
                ? clientsWithApp
                : (IReadOnlyList<AnnouncementRecipient>)[
                    .. clientsWithApp.Where(recipient => announcement.ClassGroupIds.Any(classGroupId =>
                        clientIdsByClassGroup.TryGetValue(classGroupId, out var clientIds) && clientIds.Contains(recipient.ClientId))),
                ]);
    }

    private async Task<Dictionary<Guid, HashSet<Guid>>> ClientIdsByClassGroupAsync(List<Guid> classGroupIds, CancellationToken cancellationToken)
    {
        if (classGroupIds.Count == 0)
        {
            return [];
        }

        var business = await context.Businesses.AsNoTracking()
            .FirstAsync(candidate => candidate.Id == context.CurrentTenantId, cancellationToken);
        var today = business.TodayAt(timeProvider.GetUtcNow());
        var enrolledClients = await (
            from enrollment in context.Enrollments.AsNoTracking()
            where classGroupIds.Contains(enrollment.ClassGroupId)
                && enrollment.StartDate <= today
                && (enrollment.EndDate == null || enrollment.EndDate >= today)
            join student in context.Students.AsNoTracking() on enrollment.StudentId equals student.Id
            select new { enrollment.ClassGroupId, student.ClientId })
            .Distinct()
            .ToListAsync(cancellationToken);
        return enrolledClients
            .GroupBy(enrolled => enrolled.ClassGroupId)
            .ToDictionary(group => group.Key, group => group.Select(enrolled => enrolled.ClientId).ToHashSet());
    }
}
