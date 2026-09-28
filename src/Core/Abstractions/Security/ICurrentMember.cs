namespace ClassManager.Core.Abstractions.Security;

public interface ICurrentMember
{
    Task<MemberAccess?> GetAccessAsync(CancellationToken cancellationToken);
}
