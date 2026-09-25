using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IBusinessMemberRepository
{
    void Add(BusinessMember member);

    Task<BusinessMember?> FindByUserIdAsync(Guid userId, CancellationToken cancellationToken);
}
