namespace ClassManager.Infrastructure.OnlinePayments;

public sealed class OnlinePaymentOptions
{
    public const string SectionName = "Stripe";

    public string SecretKey { get; set; } = string.Empty;
    public string ConnectWebhookSecret { get; set; } = string.Empty;
}
