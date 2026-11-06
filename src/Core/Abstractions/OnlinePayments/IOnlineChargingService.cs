using ClassManager.Core.Domain.OnlinePayments;

namespace ClassManager.Core.Abstractions.OnlinePayments;

public interface IOnlineChargingService
{
    Task<OnlinePaymentAccount?> FindChargingAccountAsync(CancellationToken cancellationToken);
}
