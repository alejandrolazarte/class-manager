using ClassManager.Core.Domain.Accounts;

namespace ClassManager.Core.Abstractions.Persistence;

public interface ILegalTermsAcceptanceRepository
{
    void Add(LegalTermsAcceptance acceptance);

    Task<bool> HasAcceptedAsync(Guid userId, string version, CancellationToken cancellationToken);
}
