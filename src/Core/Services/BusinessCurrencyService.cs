using ClassManager.Core.Abstractions.Fees;
using ClassManager.Core.Abstractions.Persistence;

namespace ClassManager.Core.Services;

public sealed class BusinessCurrencyService(IBusinessRepository businessRepository) : IBusinessCurrencyService
{
    private const string CurrentBusinessMissingMessage = "Money can only be recorded for the current business.";

    public async Task<string> CurrencyCodeAsync(CancellationToken cancellationToken)
    {
        var business = await businessRepository.GetCurrentAsync(cancellationToken)
            ?? throw new InvalidOperationException(CurrentBusinessMissingMessage);

        return business.CurrencyCode;
    }
}
