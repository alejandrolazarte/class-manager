namespace ClassManager.Core.Abstractions.Persistence;

public interface IExpiredOrderDirectory
{
    Task<IReadOnlyList<Guid>> ListBusinessesWithUnpaidRequestsCreatedBeforeAsync(DateTimeOffset createdBefore, CancellationToken cancellationToken);
}
