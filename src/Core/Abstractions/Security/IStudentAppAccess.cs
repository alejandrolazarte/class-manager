namespace ClassManager.Core.Abstractions.Security;

public sealed record StudentAppAccess(Guid AccountId, long? UserId, long BusinessId, long ClientId);

public interface IStudentAppAccess
{
    Task<StudentAppAccess?> GetAsync(CancellationToken cancellationToken);
}
