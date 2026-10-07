using ClassManager.Records;

namespace ClassManager.Core.Domain.Organizations;

public sealed class OrganizationMember : ISoftDeletable
{
    public const int RoleMaxLength = 20;

    private OrganizationMember()
    {
    }

    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid UserId { get; private set; }
    public OrganizationRole Role { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedOn { get; private set; }

    public static OrganizationMember CreateBrandOwner(Guid organizationId, Guid userId) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            OrganizationId = organizationId,
            UserId = userId,
            Role = OrganizationRole.BrandOwner,
        };

    public void Delete(DateTimeOffset deletedOn)
    {
        DeletedOn = DeletedOnGuard.Delete(DeletedOn, deletedOn);
        IsDeleted = true;
    }
}
