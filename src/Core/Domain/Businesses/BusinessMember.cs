using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Businesses;

public sealed class BusinessMember : ITenantOwned
{
    public const int RoleMaxLength = 20;

    private BusinessMember()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid UserId { get; private set; }
    public BusinessRole Role { get; private set; }

    public static BusinessMember CreateOwner(Guid businessId, Guid userId) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            TenantId = businessId,
            UserId = userId,
            Role = BusinessRole.Owner,
        };
}
