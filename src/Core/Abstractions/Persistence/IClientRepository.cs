using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IClientRepository
{
    void Add(Client client);

    Task<Client?> FindByPhoneNumberAsync(PhoneNumber phoneNumber, CancellationToken cancellationToken);

    Task<IReadOnlyList<Client>> ListByPhoneNumbersAsync(IReadOnlyCollection<PhoneNumber> phoneNumbers, CancellationToken cancellationToken);

    Task<Client?> GetByIdAsync(Guid clientId, CancellationToken cancellationToken);

    Task<Client?> GetForUpdateAsync(Guid clientId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Client>> ListAllAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<Client>> SearchAsync(ClientSearchCriteria criteria, CancellationToken cancellationToken);

    Task<bool> IsInScopeAsync(Guid clientId, ClientScope scope, CancellationToken cancellationToken);
}
