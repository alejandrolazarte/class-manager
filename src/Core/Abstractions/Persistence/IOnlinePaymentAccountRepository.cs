using ClassManager.Core.Domain.OnlinePayments;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IOnlinePaymentAccountRepository
{
    Task<OnlinePaymentAccount?> FindAsync(long organizationId, long accountId, CancellationToken cancellationToken);

    Task<OnlinePaymentAccount?> FindCreatedForAsync(long businessId, CancellationToken cancellationToken);

    Task<OnlinePaymentAccount?> FindByExternalIdAsync(string externalAccountId, CancellationToken cancellationToken);

    Task<IReadOnlyList<OnlinePaymentAccountUse>> ListOfOrganizationAsync(long organizationId, CancellationToken cancellationToken);

    void Add(OnlinePaymentAccount account);
}
