using System.Collections.Frozen;
using System.Globalization;

namespace ClassManager.Notifications.Email;

public static class EmailMoney
{
    private const string AmountCultureName = "es-AR";
    private const string WholeAmountFormat = "N0";
    private const string AmountWithCentsFormat = "N2";
    private const string UsDollarCode = "USD";
    private const string UsDollarSign = "US$";
    private const char SignSeparator = ' ';

    private static readonly CultureInfo AmountCulture = CultureInfo.GetCultureInfo(AmountCultureName);
    private static readonly Lazy<FrozenDictionary<string, string>> SignsByCurrencyCode = new(LoadSignsByCurrencyCode);

    public static string Format(decimal amount, string currencyCode) =>
        Sign(currencyCode) + SignSeparator + amount.ToString(decimal.IsInteger(amount) ? WholeAmountFormat : AmountWithCentsFormat, AmountCulture);

    private static string Sign(string currencyCode) =>
        currencyCode == UsDollarCode
            ? UsDollarSign
            : SignsByCurrencyCode.Value.GetValueOrDefault(currencyCode, currencyCode);

    private static FrozenDictionary<string, string> LoadSignsByCurrencyCode() =>
        CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            .Select(culture => new RegionInfo(culture.Name))
            .GroupBy(region => region.ISOCurrencySymbol, StringComparer.Ordinal)
            .ToFrozenDictionary(
                regions => regions.Key,
                regions => regions
                    .Select(region => region.CurrencySymbol)
                    .OrderBy(sign => sign.Length)
                    .ThenBy(sign => sign, StringComparer.Ordinal)
                    .First(),
                StringComparer.Ordinal);
}
