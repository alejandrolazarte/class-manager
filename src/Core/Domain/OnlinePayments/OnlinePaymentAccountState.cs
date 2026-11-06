namespace ClassManager.Core.Domain.OnlinePayments;

public sealed record OnlinePaymentAccountState(bool DetailsSubmitted, bool ChargesEnabled, bool PayoutsEnabled);
