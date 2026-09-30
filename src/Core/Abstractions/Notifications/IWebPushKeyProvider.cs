namespace ClassManager.Core.Abstractions.Notifications;

public interface IWebPushKeyProvider
{
    string? PublicKey { get; }
}
