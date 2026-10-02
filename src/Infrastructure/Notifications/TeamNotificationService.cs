using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Infrastructure.Persistence;
using ClassManager.Notifications.WebPush;

namespace ClassManager.Infrastructure.Notifications;

internal sealed class TeamNotificationService(AppDbContext context, TeamNotifier notifier) : ITeamNotificationService
{
    public Task AbsenceNotifiedAsync(Guid studentId, ClassGroup classGroup, ClassSession session, CancellationToken cancellationToken) =>
        NotifyInstructorAsync(studentId, classGroup, session, TeamNotificationTexts.AbsenceTitle, cancellationToken);

    public Task MakeupBookedAsync(Guid studentId, ClassGroup classGroup, ClassSession session, CancellationToken cancellationToken) =>
        NotifyInstructorAsync(studentId, classGroup, session, TeamNotificationTexts.MakeupTitle, cancellationToken);

    private async Task NotifyInstructorAsync(
        Guid studentId,
        ClassGroup classGroup,
        ClassSession session,
        Func<string, string> title,
        CancellationToken cancellationToken)
    {
        var student = await context.Students.AsNoTracking().FirstOrDefaultAsync(candidate => candidate.Id == studentId, cancellationToken);
        if (student is null)
        {
            return;
        }

        var userIds = await RecipientsAsync(session.EffectiveInstructorId(classGroup.InstructorId), cancellationToken);
        var message = new PushMessage(
            title(student.FullName),
            TeamNotificationTexts.ClassBody(classGroup.Name, session.Date, session.EffectiveStartTime(classGroup.StartTime)),
            TeamNotificationTexts.SessionUrl(classGroup.Id, session.Date));
        await notifier.NotifyAsync(userIds, message, cancellationToken);
    }

    private async Task<IReadOnlyList<Guid>> RecipientsAsync(Guid instructorId, CancellationToken cancellationToken)
    {
        var coachUserIds = await context.BusinessMembers.AsNoTracking()
            .Where(member => member.InstructorId == instructorId)
            .Select(member => member.UserId)
            .Distinct()
            .ToListAsync(cancellationToken);
        return coachUserIds.Count > 0
            ? coachUserIds
            : await context.BusinessMembers.AsNoTracking()
                .Where(member => member.Role == BusinessRole.BranchOwner)
                .Select(member => member.UserId)
                .Distinct()
                .ToListAsync(cancellationToken);
    }
}
