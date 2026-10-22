namespace ClassManager.Core.Abstractions.Security;

public sealed record StudentAppLink(long BusinessId, string BusinessName, long ClientId);

public interface IStudentAppDirectory
{
    Task<StudentAppLink?> FindDefaultAsync(Guid userId, CancellationToken cancellationToken);

    Task<StudentAppLink?> FindAsync(Guid userId, long businessId, CancellationToken cancellationToken);

    Task<IReadOnlyList<StudentAppLink>> ListAsync(Guid userId, CancellationToken cancellationToken);
}
