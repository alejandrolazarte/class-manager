using ClassManager.Tenancy.AspNetCore.Persistence;
namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class ClientInvitationRepository(AppDbContext context) : IClientInvitationRepository
{
    public void Add(ClientInvitation invitation) => context.ClientInvitations.Add(invitation);

    public Task<ClientInvitation?> FindForUpdateAsync(Guid invitationId, CancellationToken cancellationToken) =>
        context.ClientInvitations.FirstOrDefaultAsync(invitation => invitation.Id == invitationId, cancellationToken);

    public async Task<IReadOnlyList<ClientInvitation>> ListPendingForUpdateByPersonAsync(
        Guid clientId,
        Guid? studentId,
        DateTimeOffset now,
        CancellationToken cancellationToken) =>
        await context.ClientInvitations
            .Where(invitation => invitation.ClientId == clientId
                && invitation.StudentId == studentId
                && invitation.AcceptedAt == null
                && invitation.RevokedAt == null
                && invitation.DeclinedAt == null
                && invitation.ExpiresAt > now)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ClientInvitation>> ListPendingByClientAsync(Guid clientId, DateTimeOffset now, CancellationToken cancellationToken) =>
        await context.ClientInvitations
            .AsNoTracking()
            .Where(invitation => invitation.ClientId == clientId
                && invitation.AcceptedAt == null
                && invitation.RevokedAt == null
                && invitation.DeclinedAt == null
                && invitation.ExpiresAt > now)
            .OrderByDescending(invitation => invitation.CreatedAt)
            .ToListAsync(cancellationToken);

    public Task<ClientInvitation?> FindForUpdateInAnyBusinessByTokenHashAsync(string tokenHash, CancellationToken cancellationToken) =>
        context.ClientInvitations
            .IgnoreTenantFilter()
            .FirstOrDefaultAsync(invitation => invitation.TokenHash == tokenHash, cancellationToken);

    public Task<ClientInvitation?> FindForUpdateInAnyBusinessByGuardianTokenHashAsync(string guardianTokenHash, CancellationToken cancellationToken) =>
        context.ClientInvitations
            .IgnoreTenantFilter()
            .FirstOrDefaultAsync(invitation => invitation.GuardianTokenHash == guardianTokenHash, cancellationToken);
}
