namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class ClientAccountRepository(AppDbContext context) : IClientAccountRepository
{
    public void Add(ClientAccount account) => context.ClientAccounts.Add(account);

    public void Remove(ClientAccount account) => context.ClientAccounts.Remove(account);

    public async Task<IReadOnlyList<ClientAccount>> ListForUpdateByClientAsync(long clientId, CancellationToken cancellationToken) =>
        await context.ClientAccounts.Where(account => account.ClientId == clientId).ToListAsync(cancellationToken);

    public Task<ClientAccount?> FindByAccountAsync(Guid accountId, CancellationToken cancellationToken) =>
        context.ClientAccounts.AsNoTracking().FirstOrDefaultAsync(account => account.User.AccountId == accountId, cancellationToken);

    public Task<ClientAccount?> FindByAccountForUpdateAsync(Guid accountId, CancellationToken cancellationToken) =>
        context.ClientAccounts.FirstOrDefaultAsync(account => account.User.AccountId == accountId, cancellationToken);

    public Task<bool> HasAccountAsync(long clientId, long? studentId, CancellationToken cancellationToken) =>
        context.ClientAccounts.AnyAsync(account => account.ClientId == clientId && account.StudentId == studentId, cancellationToken);

    public async Task<IReadOnlyList<ClientAccount>> ListByClientAsync(long clientId, CancellationToken cancellationToken) =>
        await context.ClientAccounts
            .AsNoTracking()
            .Include(account => account.User)
            .Where(account => account.ClientId == clientId)
            .OrderBy(account => account.CreatedAt)
            .ToListAsync(cancellationToken);
}
