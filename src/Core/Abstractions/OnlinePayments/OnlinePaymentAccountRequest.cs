namespace ClassManager.Core.Abstractions.OnlinePayments;

public sealed record OnlinePaymentAccountRequest(long OrganizationId, long BusinessId, string CountryCode, string? Email);
