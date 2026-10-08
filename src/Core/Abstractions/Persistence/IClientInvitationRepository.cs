using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IClientInvitationRepository
{
    void Add(ClientInvitation invitation);

    Task<ClientInvitation?> FindForUpdateAsync(Guid invitationId, CancellationToken cancellationToken);

    Task<IReadOnlyList<ClientInvitation>> ListPendingForUpdateByPersonAsync(Guid clientId, Guid? studentId, DateTimeOffset now, CancellationToken cancellationToken);

    Task<IReadOnlyList<ClientInvitation>> ListPendingByClientAsync(Guid clientId, DateTimeOffset now, CancellationToken cancellationToken);

    Task<ClientInvitation?> FindForUpdateInAnyBusinessByTokenHashAsync(string tokenHash, CancellationToken cancellationToken);

    Task<ClientInvitation?> FindForUpdateInAnyBusinessByGuardianTokenHashAsync(string guardianTokenHash, CancellationToken cancellationToken);
}
