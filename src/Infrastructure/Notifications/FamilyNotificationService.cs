using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Infrastructure.Persistence;
using ClassManager.Infrastructure.WebPush;
using ClassManager.Notifications.WebPush;

namespace ClassManager.Infrastructure.Notifications;

internal sealed class FamilyNotificationService(AppDbContext context, PushPublisher publisher, TimeProvider timeProvider)
    : IFamilyNotificationService
{
    public Task AnnouncementPublishedAsync(Announcement announcement, CancellationToken cancellationToken)
    {
        publisher.PublishToFamilies(
            clientIds: null,
            new PushMessage(announcement.Title, FamilyPushTexts.Shorten(announcement.Body ?? string.Empty), FamilyPushTexts.NewsUrl));
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
        publisher.PublishToFamilies(
            clientIds,
            new PushMessage(
                FamilyPushTexts.ClassCancelledTitle(classGroup.Name),
                FamilyPushTexts.ClassCancelledBody(session.Date, session.EffectiveStartTime(classGroup.StartTime), session.CancellationReason),
                FamilyPushTexts.NewsUrl));
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
        publisher.PublishToFamilies(
            [student.ClientId],
            new PushMessage(
                FamilyPushTexts.FeedbackTitle(instructorFullName, student.FullName),
                FamilyPushTexts.Shorten(feedback.Text),
                FamilyPushTexts.NewsUrl));
    }
}
