using ClassManager.Core.Domain.Countries;

namespace ClassManager.Core.Domain.OnlinePayments;

public static class OnlinePaymentCountries
{
    public static IReadOnlySet<string> CountryCodes => EuropeanCountries.Codes;

    public static bool IsAvailableIn(string countryCode) => CountryCodes.Contains(countryCode);
}
