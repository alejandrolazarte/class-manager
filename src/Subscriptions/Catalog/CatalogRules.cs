namespace ClassManager.Subscriptions.Catalog;

public static class CatalogRules
{
    public const int CodeMaxLength = 40;
    public const int CurrencyLength = 3;
    public const int NoteMaxLength = 200;
    public const int PricePrecision = 18;
    public const int PriceScale = 2;
    public const int PercentPrecision = 5;
    public const int PercentScale = 2;
    public const decimal MaxPercent = 100m;

    public static string RequireCode(string? code, string parameterName)
    {
        var trimmedCode = code?.Trim() ?? string.Empty;
        if (trimmedCode.Length is 0 or > CodeMaxLength)
        {
            throw new ArgumentException($"Code must be between 1 and {CodeMaxLength} characters.", parameterName);
        }

        return trimmedCode;
    }

    public static string RequireCurrency(string? currency, string parameterName)
    {
        var upperCurrency = currency?.Trim().ToUpperInvariant() ?? string.Empty;
        if (upperCurrency.Length != CurrencyLength || !upperCurrency.All(char.IsAsciiLetterUpper))
        {
            throw new ArgumentException($"Currency must be an ISO 4217 code of {CurrencyLength} letters.", parameterName);
        }

        return upperCurrency;
    }

    public static decimal RequirePrice(decimal price, string parameterName)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(price, parameterName);
        return price;
    }

    public static decimal RequirePercent(decimal percent, string parameterName)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(percent, parameterName);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(percent, MaxPercent, parameterName);
        if (decimal.Round(percent, PercentScale) != percent)
        {
            throw new ArgumentException($"A percent has at most {PercentScale} decimals.", parameterName);
        }

        return percent;
    }

    public static bool IsPercent(decimal percent) =>
        percent is >= 0m and <= MaxPercent && decimal.Round(percent, PercentScale) == percent;

    public static int? RequireLimit(int? limit, string parameterName)
    {
        if (limit is { } value)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value, parameterName);
        }

        return limit;
    }
}
