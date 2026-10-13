using ClassManager.Core.Domain.Accounts;

namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class LegalTermsAcceptanceRepository(AppDbContext context) : ILegalTermsAcceptanceRepository
{
    public void Add(LegalTermsAcceptance acceptance) => context.LegalTermsAcceptances.Add(acceptance);

    public Task<bool> HasAcceptedAsync(Guid userId, string version, CancellationToken cancellationToken) =>
        context.LegalTermsAcceptances.AnyAsync(
            acceptance => acceptance.UserId == userId && acceptance.Version == version,
            cancellationToken);
}
