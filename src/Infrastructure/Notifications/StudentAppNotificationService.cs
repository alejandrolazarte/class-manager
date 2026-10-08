using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Infrastructure.Persistence;
using ClassManager.Infrastructure.WebPush;
using ClassManager.Notifications.WebPush;

namespace ClassManager.Infrastructure.Notifications;

internal sealed class StudentAppNotificationService(AppDbContext context, PushPublisher publisher, TimeProvider timeProvider)
    : IStudentAppNotificationService
{
    public Task AnnouncementPublishedAsync(Announcement announcement, CancellationToken cancellationToken) =>
        publisher.PublishToStudentsAsync(
            clientIds: null,
            new PushMessage(announcement.Title, StudentAppPushTexts.Shorten(announcement.Body ?? string.Empty), StudentAppPushTexts.NewsUrl),
            cancellationToken);

    public async Task GuardianConsentRequestedAsync(ClientInvitation invitation, string studentFullName, CancellationToken cancellationToken)
    {
        var clientUserIds = await context.ClientAccounts.AsNoTracking()
            .Where(account => account.ClientId == invitation.ClientId && account.StudentId == null)
            .Select(account => account.UserId)
            .ToListAsync(cancellationToken);
        await publisher.PublishToStudentAccountsAsync(
            invitation.ClientId,
            clientUserIds,
            new PushMessage(
                StudentAppPushTexts.GuardianConsentTitle(studentFullName),
                StudentAppPushTexts.GuardianConsentBody,
                StudentAppPushTexts.HomeUrl),
            cancellationToken);
    }

    public async Task ClassCancelledAsync(ClassSession session, CancellationToken cancellationToken)
    {
        var business = await context.Businesses.AsNoTracking()
            .FirstOrDefaultAsync(candidate => candidate.Id == context.CurrentTenantId, cancellationToken);
        var classGroup = await context.ClassGroups.AsNoTracking()
            .FirstOrDefaultAsync(candidate => candidate.Id == session.ClassGroupId, cancellationToken);
        if (business is null || classGroup is null || session.Date < business.TodayAt(timeProvider.GetUtcNow()))
        {
            return;
        }

        var clientIds = await (
            from enrollment in context.Enrollments.AsNoTracking()
            where enrollment.ClassGroupId == session.ClassGroupId
                && enrollment.StartDate <= session.Date
                && (enrollment.EndDate == null || enrollment.EndDate >= session.Date)
            join student in context.Students.AsNoTracking() on enrollment.StudentId equals student.Id
            select student.ClientId)
            .Distinct()
            .ToListAsync(cancellationToken);
        await publisher.PublishToStudentsAsync(
            clientIds,
            new PushMessage(
                StudentAppPushTexts.ClassCancelledTitle(classGroup.Name),
                StudentAppPushTexts.ClassCancelledBody(session.Date, session.EffectiveStartTime(classGroup.StartTime), session.CancellationReason),
                StudentAppPushTexts.NewsUrl),
            cancellationToken);
    }

    public async Task FeedbackLeftAsync(ClassFeedback feedback, CancellationToken cancellationToken)
    {
        var student = await context.Students.AsNoTracking()
            .FirstOrDefaultAsync(candidate => candidate.Id == feedback.StudentId, cancellationToken);
        if (student is null)
        {
            return;
        }

        var instructorFullName = await context.Instructors.AsNoTracking()
            .Where(instructor => instructor.Id == feedback.InstructorId)
            .Select(instructor => instructor.FullName)
            .FirstOrDefaultAsync(cancellationToken);
        await publisher.PublishToStudentsAsync(
            [student.ClientId],
            new PushMessage(
                StudentAppPushTexts.FeedbackTitle(instructorFullName, student.FullName),
                StudentAppPushTexts.Shorten(feedback.Text),
                StudentAppPushTexts.NewsUrl),
            cancellationToken);
    }
}
