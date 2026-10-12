namespace ClassManager.Core.Domain.Accounts;

public sealed record PendingBusiness(string Name, string CountryCode, string TimeZoneId, string CurrencyCode);
