using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IClientInvitationRepository
{
    void Add(ClientInvitation invitation);

    Task<IReadOnlyList<ClientInvitation>> ListPendingForUpdateByClientAsync(Guid clientId, DateTimeOffset now, CancellationToken cancellationToken);

    Task<ClientInvitation?> FindForUpdateInAnyBusinessByTokenHashAsync(string tokenHash, CancellationToken cancellationToken);
}
