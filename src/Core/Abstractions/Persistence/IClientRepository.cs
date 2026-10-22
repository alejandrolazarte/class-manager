using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IClientRepository
{
    void Add(Client client);

    Task<Client?> FindByPhoneNumberAsync(PhoneNumber phoneNumber, CancellationToken cancellationToken);

    Task<IReadOnlyList<Client>> ListByPhoneNumbersAsync(IReadOnlyCollection<PhoneNumber> phoneNumbers, CancellationToken cancellationToken);

    Task<Client?> GetByIdAsync(long clientId, CancellationToken cancellationToken);

    Task<Client?> GetForUpdateAsync(long clientId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Client>> ListAllAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<Client>> ListByIdsAsync(IReadOnlyCollection<long> clientIds, CancellationToken cancellationToken);

    Task<IReadOnlyList<Client>> SearchAsync(ClientSearchCriteria criteria, CancellationToken cancellationToken);

    Task<IReadOnlyList<long>> ListIdsInScopeAsync(ClientScope scope, CancellationToken cancellationToken);

    Task<bool> IsInScopeAsync(long clientId, ClientScope scope, CancellationToken cancellationToken);
}
