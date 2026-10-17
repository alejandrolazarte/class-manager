using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Infrastructure.Persistence;
using ClassManager.Notifications.WebPush;

namespace ClassManager.Infrastructure.Notifications;

internal sealed class TeamNotificationService(AppDbContext context, TeamNotifier notifier) : ITeamNotificationService
{
    public Task AbsenceNotifiedAsync(Guid studentId, ClassGroup classGroup, ClassSession session, CancellationToken cancellationToken) =>
        NotifyInstructorAsync(studentId, classGroup, session, (texts, name) => texts.AbsenceTitle(name), cancellationToken);

    public Task MakeupBookedAsync(Guid studentId, ClassGroup classGroup, ClassSession session, CancellationToken cancellationToken) =>
        NotifyInstructorAsync(studentId, classGroup, session, (texts, name) => texts.MakeupTitle(name), cancellationToken);

    public Task PackClassBookedAsync(Guid studentId, ClassGroup classGroup, ClassSession session, CancellationToken cancellationToken) =>
        NotifyInstructorAsync(studentId, classGroup, session, (texts, name) => texts.PackClassTitle(name), cancellationToken);

    public async Task StudentAppInvitationDeclinedAsync(ClientInvitation invitation, CancellationToken cancellationToken)
    {
        var invitedFullName = await InvitedFullNameAsync(invitation, cancellationToken);
        var message = LocalizedPushMessage.Build(texts => new PushMessage(
            texts.StudentAppInvitationDeclinedTitle(invitedFullName),
            texts.StudentAppInvitationDeclinedBody,
            TeamNotificationLinks.ClientUrl(invitation.ClientId)));
        await notifier.NotifyAsync(await InviterOrOwnersAsync(invitation.InvitedByUserId, cancellationToken), message, cancellationToken);
    }

    public async Task GuardianConsentRefusedAsync(ClientInvitation invitation, CancellationToken cancellationToken)
    {
        var invitedFullName = await InvitedFullNameAsync(invitation, cancellationToken);
        var message = LocalizedPushMessage.Build(texts => new PushMessage(
            texts.GuardianConsentRefusedTitle(invitedFullName),
            texts.GuardianConsentRefusedBody,
            TeamNotificationLinks.ClientUrl(invitation.ClientId)));
        await notifier.NotifyAsync(await InviterOrOwnersAsync(invitation.InvitedByUserId, cancellationToken), message, cancellationToken);
    }

    private async Task<string> InvitedFullNameAsync(ClientInvitation invitation, CancellationToken cancellationToken)
    {
        var fullName = invitation.StudentId is { } studentId
            ? await context.Students.AsNoTracking().Where(student => student.Id == studentId).Select(student => student.User.FullName).FirstOrDefaultAsync(cancellationToken)
            : await context.Clients.AsNoTracking().Where(client => client.Id == invitation.ClientId).Select(client => client.User.FullName).FirstOrDefaultAsync(cancellationToken);
        return fullName ?? invitation.Email;
    }

    public async Task TeamInvitationDeclinedAsync(MemberInvitation invitation, CancellationToken cancellationToken)
    {
        var message = LocalizedPushMessage.Build(texts => new PushMessage(
            texts.TeamInvitationDeclinedTitle(invitation.Email),
            texts.TeamInvitationDeclinedBody,
            TeamNotificationLinks.TeamUrl));
        await notifier.NotifyAsync(await InviterOrOwnersAsync(invitation.InvitedByUserId, cancellationToken), message, cancellationToken);
    }

    private async Task<IReadOnlyList<Guid>> InviterOrOwnersAsync(Guid? invitedByUserId, CancellationToken cancellationToken)
    {
        var inviterAccountIds = await context.BusinessMembers.AsNoTracking()
            .Where(member => invitedByUserId != null && member.UserId == invitedByUserId)
            .Select(member => member.AccountId)
            .ToListAsync(cancellationToken);
        return inviterAccountIds.Count > 0
            ? inviterAccountIds
            : await BranchOwnersAsync(cancellationToken);
    }

    private async Task<List<Guid>> BranchOwnersAsync(CancellationToken cancellationToken) =>
        await context.BusinessMembers.AsNoTracking()
            .Where(member => member.Role == BusinessRole.BranchOwner)
            .Select(member => member.AccountId)
            .Distinct()
            .ToListAsync(cancellationToken);

    private async Task NotifyInstructorAsync(
        Guid studentId,
        ClassGroup classGroup,
        ClassSession session,
        Func<INotificationTexts, string, string> title,
        CancellationToken cancellationToken)
    {
        var student = await context.Students.AsNoTracking().Include(candidate => candidate.User).FirstOrDefaultAsync(candidate => candidate.Id == studentId, cancellationToken);
        if (student is null)
        {
            return;
        }

        var userIds = await RecipientsAsync(session.EffectiveInstructorId(classGroup.InstructorId), cancellationToken);
        var message = LocalizedPushMessage.Build(texts => new PushMessage(
            title(texts, student.User.FullName),
            texts.ClassBody(classGroup.Name, session.Date, session.EffectiveStartTime(classGroup.StartTime)),
            TeamNotificationLinks.SessionUrl(classGroup.Id, session.Date)));
        await notifier.NotifyAsync(userIds, message, cancellationToken);
    }

    private async Task<IReadOnlyList<Guid>> RecipientsAsync(Guid instructorId, CancellationToken cancellationToken)
    {
        var instructorUserIds = await context.BusinessMembers.AsNoTracking()
            .Where(member => member.InstructorId == instructorId)
            .Select(member => member.AccountId)
            .Distinct()
            .ToListAsync(cancellationToken);
        return instructorUserIds.Count > 0
            ? instructorUserIds
            : await BranchOwnersAsync(cancellationToken);
    }
}
