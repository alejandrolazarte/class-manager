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
        var classGroup = await UpcomingClassGroupAsync(session, cancellationToken);
        if (classGroup is null)
        {
            return;
        }

        await publisher.PublishToStudentsAsync(
            await EnrolledClientIdsAsync(session, cancellationToken),
            new PushMessage(
                StudentAppPushTexts.ClassCancelledTitle(classGroup.Name),
                StudentAppPushTexts.ClassCancelledBody(session.Date, session.EffectiveStartTime(classGroup.StartTime), session.CancellationReason),
                StudentAppPushTexts.NewsUrl),
            cancellationToken);
    }

    public async Task ClassChangedAsync(ClassSession session, CancellationToken cancellationToken)
    {
        var classGroup = await UpcomingClassGroupAsync(session, cancellationToken);
        if (classGroup is null || !session.IsChanged)
        {
            return;
        }

        var substituteFullName = session.SubstituteInstructorId is not { } substituteId
            ? null
            : await context.Instructors.AsNoTracking()
                .Where(instructor => instructor.Id == substituteId)
                .Select(instructor => instructor.FullName)
                .FirstOrDefaultAsync(cancellationToken);
        await publisher.PublishToStudentsAsync(
            await EnrolledClientIdsAsync(session, cancellationToken),
            new PushMessage(
                StudentAppPushTexts.ClassChangedTitle(classGroup.Name),
                StudentAppPushTexts.ClassChangedBody(session.Date, session.EffectiveStartTime(classGroup.StartTime), substituteFullName),
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

    private async Task<ClassGroup?> UpcomingClassGroupAsync(ClassSession session, CancellationToken cancellationToken)
    {
        var business = await context.Businesses.AsNoTracking()
            .FirstOrDefaultAsync(candidate => candidate.Id == context.CurrentTenantId, cancellationToken);
        if (business is null || session.Date < business.TodayAt(timeProvider.GetUtcNow()))
        {
            return null;
        }

        return await context.ClassGroups.AsNoTracking()
            .FirstOrDefaultAsync(candidate => candidate.Id == session.ClassGroupId, cancellationToken);
    }

    private Task<List<Guid>> EnrolledClientIdsAsync(ClassSession session, CancellationToken cancellationToken) =>
        (
            from enrollment in context.Enrollments.AsNoTracking()
            where enrollment.ClassGroupId == session.ClassGroupId
                && enrollment.StartDate <= session.Date
                && (enrollment.EndDate == null || enrollment.EndDate >= session.Date)
            join student in context.Students.AsNoTracking() on enrollment.StudentId equals student.Id
            select student.ClientId)
            .Distinct()
            .ToListAsync(cancellationToken);
}
