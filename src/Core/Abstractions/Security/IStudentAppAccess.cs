namespace ClassManager.Core.Abstractions.Security;

public sealed record StudentAppAccess(Guid AccountId, Guid? UserId, Guid BusinessId, Guid ClientId);

public interface IStudentAppAccess
{
    Task<StudentAppAccess?> GetAsync(CancellationToken cancellationToken);
}
