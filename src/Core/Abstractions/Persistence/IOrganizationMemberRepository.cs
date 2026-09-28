using ClassManager.Core.Domain.Organizations;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IOrganizationMemberRepository
{
    void Add(OrganizationMember member);
}
