namespace ClassManager.Notifications.Email;

public interface IEmailTransport
{
    Task SendAsync(OutgoingEmail email, CancellationToken cancellationToken);
}
