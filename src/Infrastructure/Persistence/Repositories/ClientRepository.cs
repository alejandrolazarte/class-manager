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

    public async Task<IReadOnlyList<Client>> ListAllAsync(CancellationToken cancellationToken) =>
        await context.Clients.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Client>> SearchAsync(ClientSearchCriteria criteria, CancellationToken cancellationToken)
    {
        var clients = context.Clients.AsNoTracking();

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

}
