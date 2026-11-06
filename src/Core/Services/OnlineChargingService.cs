using ClassManager.Core.Abstractions.OnlinePayments;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.OnlinePayments;
using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Subscriptions.Access;

namespace ClassManager.Core.Services;

public sealed class OnlineChargingService(
    IBusinessRepository businessRepository,
    IOnlinePaymentAccountRepository accountRepository,
    IFeatureAccess featureAccess) : IOnlineChargingService
{
    public async Task<OnlinePaymentAccount?> FindChargingAccountAsync(CancellationToken cancellationToken)
    {
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        if (business?.OnlinePaymentAccountId is not { } accountId
            || !(await featureAccess.GetCurrentAsync(cancellationToken)).Has(Features.OnlinePayments))
        {
            return null;
        }

        var account = await accountRepository.FindAsync(business.OrganizationId, accountId, cancellationToken);
        return account?.Status == OnlinePaymentStatus.Active ? account : null;
    }
}
