using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class ClientRepository(AppDbContext context) : IClientRepository
{
    public void Add(Client client) => context.Clients.Add(client);

    public Task<Client?> FindByPhoneNumberAsync(PhoneNumber phoneNumber, CancellationToken cancellationToken) =>
        context.Clients.AsNoTracking().FirstOrDefaultAsync(client => client.PhoneNumber == phoneNumber, cancellationToken);

    public async Task<IReadOnlyList<Client>> ListByPhoneNumbersAsync(
        IReadOnlyCollection<PhoneNumber> phoneNumbers,
        CancellationToken cancellationToken) =>
        phoneNumbers.Count == 0
            ? []
            : await context.Clients.AsNoTracking()
                .Where(client => phoneNumbers.Contains(client.PhoneNumber))
                .ToListAsync(cancellationToken);

    public Task<Client?> GetByIdAsync(Guid clientId, CancellationToken cancellationToken) =>
        context.Clients.AsNoTracking().FirstOrDefaultAsync(client => client.Id == clientId, cancellationToken);

    public Task<Client?> GetForUpdateAsync(Guid clientId, CancellationToken cancellationToken) =>
        context.Clients.FirstOrDefaultAsync(client => client.Id == clientId, cancellationToken);

    public Task ChangeEmailInEveryBusinessByAccountUserAsync(Guid userId, string email, CancellationToken cancellationToken) =>
        context.Clients
            .IgnoreQueryFilters()
            .Where(client => context.ClientAccounts
                .IgnoreQueryFilters()
                .Any(account => account.ClientId == client.Id && account.UserId == userId))
            .ExecuteUpdateAsync(setters => setters.SetProperty(client => client.Email, email), cancellationToken);

    public async Task<IReadOnlyList<Client>> ListAllAsync(CancellationToken cancellationToken) =>
        await context.Clients.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Client>> ListByIdsAsync(IReadOnlyCollection<Guid> clientIds, CancellationToken cancellationToken)
    {
        if (clientIds.Count == 0)
        {
            return [];
        }

        return await context.Clients.AsNoTracking().Where(client => clientIds.Contains(client.Id)).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Client>> SearchAsync(ClientSearchCriteria criteria, CancellationToken cancellationToken)
    {
        var clients = context.Clients.AsNoTracking();
        if (criteria.Scope is not null)
        {
            var clientIdsInScope = ClientScopeQuery.ClientIdsIn(context, criteria.Scope);
            clients = clients.Where(client => clientIdsInScope.Contains(client.Id));
        }

        if (criteria.FullNameFragment is not null)
        {
            var fullNamePattern = LikePatterns.Contains(criteria.FullNameFragment);
            var phoneNumberPattern = LikePatterns.StartsWith(criteria.PhoneNumberPrefix);

            clients = clients.Where(client =>
                EF.Functions.Like(client.FullName, fullNamePattern, LikePatterns.EscapeCharacter)
                || (phoneNumberPattern != null
                    && EF.Functions.Like((string)(object)client.PhoneNumber, phoneNumberPattern, LikePatterns.EscapeCharacter)));
        }

        return await clients
            .OrderBy(client => client.FullName)
            .Take(criteria.Limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Guid>> ListIdsInScopeAsync(ClientScope scope, CancellationToken cancellationToken) =>
        await ClientScopeQuery.ClientIdsIn(context, scope).Distinct().ToListAsync(cancellationToken);

    public Task<bool> IsInScopeAsync(Guid clientId, ClientScope scope, CancellationToken cancellationToken) =>
        ClientScopeQuery.ClientIdsIn(context, scope).AnyAsync(clientIdInScope => clientIdInScope == clientId, cancellationToken);
}
