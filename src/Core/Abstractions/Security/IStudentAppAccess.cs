namespace ClassManager.Core.Abstractions.Security;

public sealed record StudentAppAccess(Guid AccountId, long? UserId, long BusinessId, long ClientId, long? StudentId = null)
{
    public bool PaysForFamily => StudentId is null;
}

public interface IStudentAppAccess
{
    Task<StudentAppAccess?> GetAsync(CancellationToken cancellationToken);
}
