namespace ClassManager.Core.Abstractions.Security;

public sealed record StudentAppLink(Guid BusinessId, string BusinessName, Guid ClientId);

public interface IStudentAppDirectory
{
    Task<StudentAppLink?> FindDefaultAsync(Guid userId, CancellationToken cancellationToken);

    Task<StudentAppLink?> FindAsync(Guid userId, Guid businessId, CancellationToken cancellationToken);

    Task<IReadOnlyList<StudentAppLink>> ListAsync(Guid userId, CancellationToken cancellationToken);
}
