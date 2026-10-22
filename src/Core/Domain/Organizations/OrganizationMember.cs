using ClassManager.Records;

namespace ClassManager.Core.Domain.Organizations;

public sealed class OrganizationMember : ISoftDeletable
{
    public const int RoleMaxLength = 20;

    private OrganizationMember()
    {
    }

    public long Id { get; private set; }
    public long OrganizationId { get; private set; }
    public Guid UserId { get; private set; }
    public OrganizationRole Role { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public long? DeletedByUserId { get; private set; }

    public static OrganizationMember CreateBrandOwner(long organizationId, Guid userId) =>
        new()
        {
            OrganizationId = organizationId,
            UserId = userId,
            Role = OrganizationRole.BrandOwner,
        };

    public void Delete(DateTimeOffset deletedAt)
    {
        DeletedAt = SoftDeleteGuard.Delete(DeletedAt, deletedAt);
        IsDeleted = true;
    }
}
