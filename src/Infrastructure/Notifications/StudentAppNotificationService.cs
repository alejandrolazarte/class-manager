using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Infrastructure.Persistence;
using ClassManager.Infrastructure.WebPush;
using ClassManager.Notifications.WebPush;

namespace ClassManager.Infrastructure.Notifications;

internal sealed class StudentAppNotificationService(AppDbContext context, PushPublisher publisher, TimeProvider timeProvider)
    : IStudentAppNotificationService
{
    public Task AnnouncementPublishedAsync(Announcement announcement, CancellationToken cancellationToken)
    {
        publisher.PublishToStudents(
            clientIds: null,
            new PushMessage(announcement.Title, StudentAppPushTexts.Shorten(announcement.Body ?? string.Empty), StudentAppPushTexts.NewsUrl));
        return Task.CompletedTask;
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
        publisher.PublishToStudents(
            clientIds,
            new PushMessage(
                StudentAppPushTexts.ClassCancelledTitle(classGroup.Name),
                StudentAppPushTexts.ClassCancelledBody(session.Date, session.EffectiveStartTime(classGroup.StartTime), session.CancellationReason),
                StudentAppPushTexts.NewsUrl));
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
        publisher.PublishToStudents(
            [student.ClientId],
            new PushMessage(
                StudentAppPushTexts.FeedbackTitle(instructorFullName, student.FullName),
                StudentAppPushTexts.Shorten(feedback.Text),
                StudentAppPushTexts.NewsUrl));
    }
}
