using ClassManager.Core.Domain.Announcements;
using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.Abstractions.Notifications;

public interface IStudentAppNotificationService
{
    Task AnnouncementPublishedAsync(Announcement announcement, CancellationToken cancellationToken);

    Task ClassCancelledAsync(ClassSession session, CancellationToken cancellationToken);

    Task FeedbackLeftAsync(ClassFeedback feedback, CancellationToken cancellationToken);
}
