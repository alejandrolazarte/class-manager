namespace ClassManager.Core.Abstractions.Security;

public sealed record FamilyLink(Guid BusinessId, string BusinessName, Guid ClientId);

public interface IFamilyDirectory
{
    Task<FamilyLink?> FindDefaultAsync(Guid userId, CancellationToken cancellationToken);

    Task<FamilyLink?> FindAsync(Guid userId, Guid businessId, CancellationToken cancellationToken);
}
