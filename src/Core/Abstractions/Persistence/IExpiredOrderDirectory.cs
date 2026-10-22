namespace ClassManager.Core.Abstractions.Persistence;

public interface IExpiredOrderDirectory
{
    Task<IReadOnlyList<long>> ListBusinessesWithUnpaidRequestsCreatedBeforeAsync(DateTimeOffset createdBefore, CancellationToken cancellationToken);
}
