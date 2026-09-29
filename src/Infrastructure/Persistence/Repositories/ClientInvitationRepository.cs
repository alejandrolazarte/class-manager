namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class ClientInvitationRepository(AppDbContext context) : IClientInvitationRepository
{
    public void Add(ClientInvitation invitation) => context.ClientInvitations.Add(invitation);

    public async Task<IReadOnlyList<ClientInvitation>> ListPendingForUpdateByClientAsync(
        Guid clientId,
        DateTimeOffset now,
        CancellationToken cancellationToken) =>
        await context.ClientInvitations
            .Where(invitation => invitation.ClientId == clientId
                && invitation.AcceptedAt == null
                && invitation.RevokedAt == null
                && invitation.ExpiresAt > now)
            .ToListAsync(cancellationToken);

    public Task<ClientInvitation?> FindForUpdateInAnyBusinessByTokenHashAsync(string tokenHash, CancellationToken cancellationToken) =>
        context.ClientInvitations
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(invitation => invitation.TokenHash == tokenHash, cancellationToken);
}
