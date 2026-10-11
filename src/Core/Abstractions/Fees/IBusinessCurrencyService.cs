namespace ClassManager.Core.Abstractions.Fees;

public interface IBusinessCurrencyService
{
    Task<string> CurrencyCodeAsync(CancellationToken cancellationToken);
}
