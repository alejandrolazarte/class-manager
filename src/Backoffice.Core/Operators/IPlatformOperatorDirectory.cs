namespace ClassManager.Backoffice.Core.Operators;

public interface IPlatformOperatorDirectory
{
    Task<PlatformOperator?> FindAsync(Guid accountId, CancellationToken cancellationToken);
}
