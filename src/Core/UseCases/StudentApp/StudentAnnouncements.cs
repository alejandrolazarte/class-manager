using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.Announcements;

namespace ClassManager.Core.UseCases.StudentApp;

internal static class StudentAnnouncements
{
    public const int ListedDays = 60;

    public static async Task<HashSet<long>> EnrolledClassGroupIdsAsync(
        IEnrollmentRepository enrollmentRepository, IEnumerable<long> studentIds, DateOnly today, CancellationToken cancellationToken)
    {
        var classGroupIds = new HashSet<long>();
        foreach (var studentId in studentIds)
        {
            classGroupIds.UnionWith((await enrollmentRepository.ListCurrentByStudentAsync(studentId, today, cancellationToken))
                .Select(enrollment => enrollment.ClassGroupId));
        }

        return classGroupIds;
    }

    public static async Task<IReadOnlyList<Announcement>> VisibleAsync(
        IAnnouncementRepository announcementRepository,
        IReadOnlyCollection<long> enrolledClassGroupIds,
        DateOnly today,
        DateTimeOffset now,
        CancellationToken cancellationToken) =>
        [
            .. (await announcementRepository.ListRecentOrPinnedAsync(now.AddDays(-ListedDays), cancellationToken))
                .Where(announcement => announcement.IsPublishedBy(now)
                    && announcement.IsVisibleOn(today)
                    && announcement.IsFor(enrolledClassGroupIds)),
        ];
}
