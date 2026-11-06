namespace ClassManager.Core.Abstractions.OnlinePayments;

public interface IOnlinePaymentWebhookReader
{
    OnlinePaymentEvent? Read(string payload, string signatureHeader);
}
