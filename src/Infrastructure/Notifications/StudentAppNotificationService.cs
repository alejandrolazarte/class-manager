using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Infrastructure.BackgroundTasks;
using ClassManager.Infrastructure.Persistence;
using ClassManager.Infrastructure.WebPush;
using ClassManager.Notifications.WebPush;

namespace ClassManager.Infrastructure.Notifications;

internal sealed class StudentAppNotificationService(
    AppDbContext context,
    PushPublisher publisher,
    IBackgroundTaskOutbox backgroundTasks,
    TimeProvider timeProvider)
    : IStudentAppNotificationService
{
    public async Task AnnouncementPublishedAsync(Announcement announcement, CancellationToken cancellationToken)
    {
        if (!announcement.IsPublishedBy(timeProvider.GetUtcNow()))
        {
            await backgroundTasks.EnqueueAsync(
                new PublishScheduledAnnouncementBackgroundTaskCommand(context.CurrentTenantId, announcement.Id),
                announcement.PublishedAt,
                cancellationToken);
            return;
        }

        await publisher.PublishToStudentsAsync(
            announcement.IsForEveryone ? null : await ClientIdsEnrolledInAsync(announcement.ClassGroupIds, cancellationToken),
            LocalizedPushMessage.SameInEveryLanguage(
                new PushMessage(announcement.Title, PushBodies.Shorten(announcement.Body ?? string.Empty), StudentAppPushLinks.NewsUrl)),
            cancellationToken);
    }

    public async Task AnnouncementRescheduledAsync(Announcement announcement, CancellationToken cancellationToken)
    {
        await backgroundTasks.CancelPendingAsync(
            new PublishScheduledAnnouncementBackgroundTaskCommand(context.CurrentTenantId, announcement.Id), cancellationToken);
        await AnnouncementPublishedAsync(announcement, cancellationToken);
    }

    public Task AnnouncementReminderAsync(Announcement announcement, IReadOnlyCollection<Guid> clientIds, CancellationToken cancellationToken) =>
        publisher.PublishToStudentsAsync(
            clientIds,
            LocalizedPushMessage.Build(texts => new PushMessage(
                texts.AnnouncementReminderTitle(announcement.Title),
                PushBodies.Shorten(announcement.Body ?? string.Empty),
                StudentAppPushLinks.NewsUrl)),
            cancellationToken);

    public async Task GuardianConsentRequestedAsync(ClientInvitation invitation, string studentFullName, CancellationToken cancellationToken)
    {
        var clientUserIds = await context.ClientAccounts.AsNoTracking()
            .Where(account => account.ClientId == invitation.ClientId && account.StudentId == null)
            .Select(account => account.AccountId)
            .ToListAsync(cancellationToken);
        await publisher.PublishToStudentAccountsAsync(
            invitation.ClientId,
            clientUserIds,
            LocalizedPushMessage.Build(texts => new PushMessage(
                texts.GuardianConsentTitle(studentFullName),
                texts.GuardianConsentBody,
                StudentAppPushLinks.HomeUrl)),
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
            LocalizedPushMessage.Build(texts => new PushMessage(
                texts.ClassCancelledTitle(classGroup.Name),
                texts.ClassCancelledBody(session.Date, session.EffectiveStartTime(classGroup.StartTime), session.CancellationReason),
                StudentAppPushLinks.NewsUrl)),
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
            LocalizedPushMessage.Build(texts => new PushMessage(
                texts.ClassChangedTitle(classGroup.Name),
                texts.ClassChangedBody(session.Date, session.EffectiveStartTime(classGroup.StartTime), substituteFullName),
                StudentAppPushLinks.NewsUrl)),
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
            LocalizedPushMessage.Build(texts => new PushMessage(
                texts.FeedbackTitle(instructorFullName, student.FullName),
                PushBodies.Shorten(feedback.Text),
                StudentAppPushLinks.NewsUrl)),
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

    private async Task<List<Guid>> ClientIdsEnrolledInAsync(IReadOnlyList<Guid> classGroupIds, CancellationToken cancellationToken)
    {
        var business = await context.Businesses.AsNoTracking()
            .FirstAsync(candidate => candidate.Id == context.CurrentTenantId, cancellationToken);
        var today = business.TodayAt(timeProvider.GetUtcNow());
        return await (
            from enrollment in context.Enrollments.AsNoTracking()
            where classGroupIds.Contains(enrollment.ClassGroupId)
                && enrollment.StartDate <= today
                && (enrollment.EndDate == null || enrollment.EndDate >= today)
            join student in context.Students.AsNoTracking() on enrollment.StudentId equals student.Id
            select student.ClientId)
            .Distinct()
            .ToListAsync(cancellationToken);
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
