namespace ClassManager.Core.Abstractions.Security;

public interface ICurrentUser
{
    Guid? AccountId { get; }

    Guid? UserId { get; }
}
