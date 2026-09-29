namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class ClientAccountRepository(AppDbContext context) : IClientAccountRepository
{
    public void Add(ClientAccount account) => context.ClientAccounts.Add(account);

    public Task<ClientAccount?> FindByUserAsync(Guid userId, CancellationToken cancellationToken) =>
        context.ClientAccounts.AsNoTracking().FirstOrDefaultAsync(account => account.UserId == userId, cancellationToken);

    public Task<bool> HasAccountAsync(Guid clientId, CancellationToken cancellationToken) =>
        context.ClientAccounts.AnyAsync(account => account.ClientId == clientId, cancellationToken);
}
