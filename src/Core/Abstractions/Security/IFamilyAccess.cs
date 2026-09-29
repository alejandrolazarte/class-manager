namespace ClassManager.Core.Abstractions.Security;

public sealed record FamilyAccess(Guid UserId, Guid BusinessId, Guid ClientId);

public interface IFamilyAccess
{
    Task<FamilyAccess?> GetAsync(CancellationToken cancellationToken);
}
