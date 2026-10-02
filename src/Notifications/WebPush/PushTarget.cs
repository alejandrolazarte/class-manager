namespace ClassManager.Notifications.WebPush;

public sealed record PushTarget(string Endpoint, string P256dh, string Auth);
