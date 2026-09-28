namespace ClassManager.Core.Domain.Organizations;

public sealed class OrganizationMember
{
    public const int RoleMaxLength = 20;

    private OrganizationMember()
    {
    }

    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid UserId { get; private set; }
    public OrganizationRole Role { get; private set; }

    public static OrganizationMember CreateBrandOwner(Guid organizationId, Guid userId) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            OrganizationId = organizationId,
            UserId = userId,
            Role = OrganizationRole.BrandOwner,
        };
}
