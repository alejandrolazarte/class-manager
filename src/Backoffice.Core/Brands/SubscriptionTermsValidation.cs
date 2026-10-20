using ClassManager.Core.Common;
using ClassManager.Subscriptions.Catalog;

namespace ClassManager.Backoffice.Core.Brands;

public static class SubscriptionTermsValidation
{
    private const string PriceField = "Price";
    private const string CurrencyField = "Currency";
    private const string ExpiredOnField = "ExpiredOn";

    public static ResultError? Validate(decimal price, string? currency, DateOnly? expiredOn, DateOnly today)
    {
        if (price < 0)
        {
            return Error(BrandErrorCodes.NegativePriceMessage, BrandErrorCodes.NegativePrice, PriceField);
        }

        var trimmedCurrency = currency?.Trim() ?? string.Empty;
        if (trimmedCurrency.Length != CatalogRules.CurrencyLength || !trimmedCurrency.All(char.IsAsciiLetter))
        {
            return Error(BrandErrorCodes.InvalidCurrencyMessage, BrandErrorCodes.InvalidCurrency, CurrencyField);
        }

        return expiredOn < today
            ? Error(BrandErrorCodes.EndDateInThePastMessage, BrandErrorCodes.EndDateInThePast, ExpiredOnField)
            : null;
    }

    public static ResultError Error(string message, string code, string fieldName) =>
        new(code, message, ErrorKind.Validation) { FieldName = fieldName };
}
