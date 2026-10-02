namespace ClassManager.Notifications.WebPush;

public interface IWebPushSender
{
    Task<PushDelivery> SendAsync(PushTarget target, string payload, CancellationToken cancellationToken);
}
