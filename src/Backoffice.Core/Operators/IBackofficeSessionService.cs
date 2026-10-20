namespace ClassManager.Backoffice.Core.Operators;

public interface IBackofficeSessionService
{
    Task RevokeAllAsync(Guid accountId, CancellationToken cancellationToken);
}
