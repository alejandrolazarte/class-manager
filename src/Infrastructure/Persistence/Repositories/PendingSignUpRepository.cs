using ClassManager.Core.Domain.Accounts;

namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class PendingSignUpRepository(AppDbContext context) : IPendingSignUpRepository
{
    public void Add(PendingSignUp pendingSignUp) => context.PendingSignUps.Add(pendingSignUp);

    public async Task<IReadOnlyList<PendingSignUp>> ListPendingForUpdateByEmailAsync(
        string email,
        DateTimeOffset now,
        CancellationToken cancellationToken) =>
        await context.PendingSignUps
            .Where(pendingSignUp => pendingSignUp.Email == email
                && pendingSignUp.CompletedAt == null
                && pendingSignUp.RevokedAt == null
                && pendingSignUp.ExpiresAt > now)
            .ToListAsync(cancellationToken);

    public Task<PendingSignUp?> FindForUpdateByTokenHashAsync(string tokenHash, CancellationToken cancellationToken) =>
        context.PendingSignUps.FirstOrDefaultAsync(pendingSignUp => pendingSignUp.TokenHash == tokenHash, cancellationToken);
}
