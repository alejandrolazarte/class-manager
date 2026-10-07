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

    public Task PackClassBookedAsync(Guid studentId, ClassGroup classGroup, ClassSession session, CancellationToken cancellationToken) =>
        NotifyInstructorAsync(studentId, classGroup, session, TeamNotificationTexts.PackClassTitle, cancellationToken);

    public async Task StudentAppInvitationDeclinedAsync(ClientInvitation invitation, CancellationToken cancellationToken)
    {
        var message = new PushMessage(
            TeamNotificationTexts.StudentAppInvitationDeclinedTitle(await InvitedFullNameAsync(invitation, cancellationToken)),
            TeamNotificationTexts.StudentAppInvitationDeclinedBody,
            TeamNotificationTexts.ClientUrl(invitation.ClientId));
        await notifier.NotifyAsync(await InviterOrOwnersAsync(invitation.InvitedByUserId, cancellationToken), message, cancellationToken);
    }

    public async Task GuardianConsentRefusedAsync(ClientInvitation invitation, CancellationToken cancellationToken)
    {
        var message = new PushMessage(
            TeamNotificationTexts.GuardianConsentRefusedTitle(await InvitedFullNameAsync(invitation, cancellationToken)),
            TeamNotificationTexts.GuardianConsentRefusedBody,
            TeamNotificationTexts.ClientUrl(invitation.ClientId));
        await notifier.NotifyAsync(await InviterOrOwnersAsync(invitation.InvitedByUserId, cancellationToken), message, cancellationToken);
    }

    private async Task<string> InvitedFullNameAsync(ClientInvitation invitation, CancellationToken cancellationToken)
    {
        var fullName = invitation.StudentId is { } studentId
            ? await context.Students.AsNoTracking().Where(student => student.Id == studentId).Select(student => student.FullName).FirstOrDefaultAsync(cancellationToken)
            : await context.Clients.AsNoTracking().Where(client => client.Id == invitation.ClientId).Select(client => client.FullName).FirstOrDefaultAsync(cancellationToken);
        return fullName ?? invitation.Email;
    }

    public async Task TeamInvitationDeclinedAsync(MemberInvitation invitation, CancellationToken cancellationToken)
    {
        var message = new PushMessage(
            TeamNotificationTexts.TeamInvitationDeclinedTitle(invitation.Email),
            TeamNotificationTexts.TeamInvitationDeclinedBody,
            TeamNotificationTexts.TeamUrl);
        await notifier.NotifyAsync(await InviterOrOwnersAsync(invitation.InvitedByUserId, cancellationToken), message, cancellationToken);
    }

    private async Task<IReadOnlyList<Guid>> InviterOrOwnersAsync(Guid invitedByUserId, CancellationToken cancellationToken) =>
        await context.BusinessMembers.AsNoTracking().AnyAsync(member => member.UserId == invitedByUserId, cancellationToken)
            ? [invitedByUserId]
            : await BranchOwnersAsync(cancellationToken);

    private async Task<List<Guid>> BranchOwnersAsync(CancellationToken cancellationToken) =>
        await context.BusinessMembers.AsNoTracking()
            .Where(member => member.Role == BusinessRole.BranchOwner)
            .Select(member => member.UserId)
            .Distinct()
            .ToListAsync(cancellationToken);

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
            : await BranchOwnersAsync(cancellationToken);
    }
}
