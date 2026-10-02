namespace ClassManager.Infrastructure.Email;

public interface IEmailTransport
{
    Task SendAsync(OutgoingEmail email, CancellationToken cancellationToken);
}
