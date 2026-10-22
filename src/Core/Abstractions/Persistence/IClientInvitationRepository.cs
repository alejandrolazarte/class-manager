using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IClientInvitationRepository
{
    void Add(ClientInvitation invitation);

    Task<ClientInvitation?> FindForUpdateAsync(long invitationId, CancellationToken cancellationToken);

    Task<IReadOnlyList<ClientInvitation>> ListPendingForUpdateByPersonAsync(long clientId, long? studentId, DateTimeOffset now, CancellationToken cancellationToken);

    Task<IReadOnlyList<ClientInvitation>> ListPendingByClientAsync(long clientId, DateTimeOffset now, CancellationToken cancellationToken);

    Task<ClientInvitation?> FindForUpdateInAnyBusinessByTokenHashAsync(string tokenHash, CancellationToken cancellationToken);

    Task<ClientInvitation?> FindForUpdateInAnyBusinessByGuardianTokenHashAsync(string guardianTokenHash, CancellationToken cancellationToken);
}
