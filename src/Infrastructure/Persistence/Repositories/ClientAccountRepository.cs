namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class ClientAccountRepository(AppDbContext context) : IClientAccountRepository
{
    public void Add(ClientAccount account) => context.ClientAccounts.Add(account);

    public Task<ClientAccount?> FindByUserAsync(Guid userId, CancellationToken cancellationToken) =>
        context.ClientAccounts.AsNoTracking().FirstOrDefaultAsync(account => account.UserId == userId, cancellationToken);

    public Task<ClientAccount?> FindByUserForUpdateAsync(Guid userId, CancellationToken cancellationToken) =>
        context.ClientAccounts.FirstOrDefaultAsync(account => account.UserId == userId, cancellationToken);

    public Task<bool> HasAccountAsync(Guid clientId, Guid? studentId, CancellationToken cancellationToken) =>
        context.ClientAccounts.AnyAsync(account => account.ClientId == clientId && account.StudentId == studentId, cancellationToken);

    public async Task<IReadOnlyList<ClientAccount>> ListByClientAsync(Guid clientId, CancellationToken cancellationToken) =>
        await context.ClientAccounts
            .AsNoTracking()
            .Where(account => account.ClientId == clientId)
            .OrderBy(account => account.CreatedAt)
            .ToListAsync(cancellationToken);
}
