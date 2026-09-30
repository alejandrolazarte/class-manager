using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IClientAccountRepository
{
    void Add(ClientAccount account);

    Task<ClientAccount?> FindByUserAsync(Guid userId, CancellationToken cancellationToken);

    Task<ClientAccount?> FindByUserForUpdateAsync(Guid userId, CancellationToken cancellationToken);

    Task<bool> HasAccountAsync(Guid clientId, CancellationToken cancellationToken);
}
