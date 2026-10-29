using ClassManager.Core.Domain.Documents;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Users;

public sealed class UserPhoto : ITenantOwned, IAuditable
{
    public const int SideInPixels = 256;
    public const int MaximumUploadSizeInBytes = 10 * 1024 * 1024;

    private UserPhoto()
    {
    }

    public long Id { get; private set; }
    public long TenantId { get; private set; }
    public long UserId { get; private set; }
    public long DocumentId { get; private set; }
    public Document Document { get; private set; } = null!;
    public bool AwaitsGuardian { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public long? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long? UpdatedByUserId { get; private set; }

    public static UserPhoto Create(long userId, Document document, bool awaitsGuardian) =>
        new()
        {
            UserId = userId,
            DocumentId = document.Id,
            Document = document,
            AwaitsGuardian = awaitsGuardian,
        };

    public void ApproveByGuardian() => AwaitsGuardian = false;
}
