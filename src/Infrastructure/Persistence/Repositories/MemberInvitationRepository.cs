namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class MemberInvitationRepository(AppDbContext context) : IMemberInvitationRepository
{
    public void Add(MemberInvitation invitation) => context.MemberInvitations.Add(invitation);

    public Task<MemberInvitation?> GetForUpdateAsync(Guid invitationId, CancellationToken cancellationToken) =>
        context.MemberInvitations.FirstOrDefaultAsync(invitation => invitation.Id == invitationId, cancellationToken);

    public async Task<IReadOnlyList<MemberInvitation>> ListPendingAsync(DateTimeOffset now, CancellationToken cancellationToken) =>
        await PendingAt(context.MemberInvitations.AsNoTracking(), now).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<MemberInvitation>> ListPendingByCustomRoleAsync(
        Guid customRoleId,
        DateTimeOffset now,
        CancellationToken cancellationToken) =>
        await PendingAt(context.MemberInvitations.AsNoTracking(), now)
            .Where(invitation => invitation.CustomRoleId == customRoleId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<MemberInvitation>> ListPendingForUpdateByEmailAsync(
        string email,
        DateTimeOffset now,
        CancellationToken cancellationToken) =>
        await PendingAt(context.MemberInvitations, now)
            .Where(invitation => invitation.Email == email)
            .ToListAsync(cancellationToken);

    public Task<MemberInvitation?> FindForUpdateInAnyBusinessByTokenHashAsync(string tokenHash, CancellationToken cancellationToken) =>
        context.MemberInvitations
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(invitation => invitation.TokenHash == tokenHash, cancellationToken);

    private static IQueryable<MemberInvitation> PendingAt(IQueryable<MemberInvitation> invitations, DateTimeOffset now) =>
        invitations.Where(invitation => invitation.AcceptedAt == null && invitation.RevokedAt == null && invitation.ExpiresAt > now);
}
