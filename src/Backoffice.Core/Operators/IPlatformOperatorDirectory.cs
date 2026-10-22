using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Backoffice.Core.Operators;

public interface IPlatformOperatorDirectory
{
    Task<PlatformOperator?> FindAsync(Guid accountId, CancellationToken cancellationToken);

    Task<IReadOnlyList<PlatformTeamMember>> ListTeamAsync(CancellationToken cancellationToken);

    Task<BusinessMember?> FindTeamMemberAsync(long memberId, CancellationToken cancellationToken);
}
