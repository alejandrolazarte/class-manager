using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.Abstractions.Notifications;

public interface ITeamNotificationService
{
    Task AbsenceNotifiedAsync(Guid studentId, ClassGroup classGroup, ClassSession session, CancellationToken cancellationToken);

    Task MakeupBookedAsync(Guid studentId, ClassGroup classGroup, ClassSession session, CancellationToken cancellationToken);

    Task PackClassBookedAsync(Guid studentId, ClassGroup classGroup, ClassSession session, CancellationToken cancellationToken);

    Task StudentAppInvitationDeclinedAsync(ClientInvitation invitation, CancellationToken cancellationToken);

    Task TeamInvitationDeclinedAsync(MemberInvitation invitation, CancellationToken cancellationToken);
}
