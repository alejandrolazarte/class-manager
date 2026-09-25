namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class ClientRepository(AppDbContext context) : IClientRepository
{
    private const char LikeWildcard = '%';
    private const string LikeEscapeCharacter = "\\";

    public void Add(Client client) => context.Clients.Add(client);

    public Task<Client?> FindByPhoneNumberAsync(PhoneNumber phoneNumber, CancellationToken cancellationToken) =>
        context.Clients.AsNoTracking().FirstOrDefaultAsync(client => client.PhoneNumber == phoneNumber, cancellationToken);

    public Task<Client?> GetByIdAsync(Guid clientId, CancellationToken cancellationToken) =>
        context.Clients.AsNoTracking().FirstOrDefaultAsync(client => client.Id == clientId, cancellationToken);

    public async Task<IReadOnlyList<Client>> SearchAsync(ClientSearchCriteria criteria, CancellationToken cancellationToken)
    {
        var clients = context.Clients.AsNoTracking();

        if (criteria.FullNameFragment is not null)
        {
            var fullNamePattern = LikeWildcard + EscapeLikePattern(criteria.FullNameFragment) + LikeWildcard;
            var phoneNumberPattern = criteria.PhoneNumberPrefix is null
                ? null
                : EscapeLikePattern(criteria.PhoneNumberPrefix) + LikeWildcard;

            clients = clients.Where(client =>
                EF.Functions.Like(client.FullName, fullNamePattern, LikeEscapeCharacter)
                || (phoneNumberPattern != null
                    && EF.Functions.Like((string)(object)client.PhoneNumber, phoneNumberPattern, LikeEscapeCharacter)));
        }

        return await clients
            .OrderBy(client => client.FullName)
            .Take(criteria.Limit)
            .ToListAsync(cancellationToken);
    }

    private static string EscapeLikePattern(string value) =>
        value
            .Replace(LikeEscapeCharacter, LikeEscapeCharacter + LikeEscapeCharacter, StringComparison.Ordinal)
            .Replace("%", LikeEscapeCharacter + "%", StringComparison.Ordinal)
            .Replace("_", LikeEscapeCharacter + "_", StringComparison.Ordinal)
            .Replace("[", LikeEscapeCharacter + "[", StringComparison.Ordinal);
}
