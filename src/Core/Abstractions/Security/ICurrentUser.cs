namespace ClassManager.Core.Abstractions.Security;

public interface ICurrentUser
{
    Guid? UserId { get; }
}
