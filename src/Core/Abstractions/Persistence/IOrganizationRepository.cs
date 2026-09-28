using ClassManager.Core.Domain.Organizations;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IOrganizationRepository
{
    void Add(Organization organization);
}
