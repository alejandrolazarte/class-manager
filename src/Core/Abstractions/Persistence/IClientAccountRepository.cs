using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IClientAccountRepository
{
    void Add(ClientAccount account);

    void Remove(ClientAccount account);

    Task<IReadOnlyList<ClientAccount>> ListForUpdateByClientAsync(Guid clientId, CancellationToken cancellationToken);

    Task<ClientAccount?> FindByAccountAsync(Guid accountId, CancellationToken cancellationToken);

    Task<ClientAccount?> FindByAccountForUpdateAsync(Guid accountId, CancellationToken cancellationToken);

    Task<bool> HasAccountAsync(Guid clientId, Guid? studentId, CancellationToken cancellationToken);

    Task<IReadOnlyList<ClientAccount>> ListByClientAsync(Guid clientId, CancellationToken cancellationToken);
}
