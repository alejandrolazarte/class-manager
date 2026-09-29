using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IBusinessMemberRepository
{
    void Add(BusinessMember member);

    void Remove(BusinessMember member);

    Task<IReadOnlyList<BusinessMember>> ListAsync(CancellationToken cancellationToken);

    Task<BusinessMember?> GetForUpdateAsync(Guid memberId, CancellationToken cancellationToken);

    Task<bool> IsUserMemberAsync(Guid userId, CancellationToken cancellationToken);

    Task<IReadOnlyList<BusinessMember>> ListByCustomRoleAsync(Guid customRoleId, CancellationToken cancellationToken);

    Task<bool> IsInstructorLinkedAsync(Guid instructorId, Guid? exceptMemberId, CancellationToken cancellationToken);
}
