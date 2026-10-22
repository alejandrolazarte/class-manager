using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IClientAccountRepository
{
    void Add(ClientAccount account);

    void Remove(ClientAccount account);

    Task<IReadOnlyList<ClientAccount>> ListForUpdateByClientAsync(long clientId, CancellationToken cancellationToken);

    Task<ClientAccount?> FindByAccountAsync(Guid accountId, CancellationToken cancellationToken);

    Task<ClientAccount?> FindByAccountForUpdateAsync(Guid accountId, CancellationToken cancellationToken);

    Task<bool> HasAccountAsync(long clientId, long? studentId, CancellationToken cancellationToken);

    Task<IReadOnlyList<ClientAccount>> ListByClientAsync(long clientId, CancellationToken cancellationToken);
}
