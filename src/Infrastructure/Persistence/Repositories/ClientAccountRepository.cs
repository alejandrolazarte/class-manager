namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class ClientAccountRepository(AppDbContext context) : IClientAccountRepository
{
    public void Add(ClientAccount account) => context.ClientAccounts.Add(account);

    public Task<ClientAccount?> FindByAccountAsync(Guid accountId, CancellationToken cancellationToken) =>
        context.ClientAccounts.AsNoTracking().FirstOrDefaultAsync(account => account.User.AccountId == accountId, cancellationToken);

    public Task<ClientAccount?> FindByAccountForUpdateAsync(Guid accountId, CancellationToken cancellationToken) =>
        context.ClientAccounts.FirstOrDefaultAsync(account => account.User.AccountId == accountId, cancellationToken);

    public Task<bool> HasAccountAsync(Guid clientId, Guid? studentId, CancellationToken cancellationToken) =>
        context.ClientAccounts.AnyAsync(account => account.ClientId == clientId && account.StudentId == studentId, cancellationToken);

    public async Task<IReadOnlyList<ClientAccount>> ListByClientAsync(Guid clientId, CancellationToken cancellationToken) =>
        await context.ClientAccounts
            .AsNoTracking()
            .Include(account => account.User)
            .Where(account => account.ClientId == clientId)
            .OrderBy(account => account.CreatedAt)
            .ToListAsync(cancellationToken);
}
