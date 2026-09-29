namespace ClassManager.Core.Abstractions.Persistence;

public interface IExpiredOrderDirectory
{
    Task<IReadOnlyList<Guid>> ListBusinessesWithRequestsCreatedBeforeAsync(DateTimeOffset createdBefore, CancellationToken cancellationToken);
}
