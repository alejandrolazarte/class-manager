using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.Abstractions.Notifications;

public interface ITeamNotificationService
{
    Task AbsenceNotifiedAsync(Guid studentId, ClassGroup classGroup, ClassSession session, CancellationToken cancellationToken);

    Task MakeupBookedAsync(Guid studentId, ClassGroup classGroup, ClassSession session, CancellationToken cancellationToken);
}
