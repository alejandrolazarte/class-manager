using ClassManager.Core.Domain.Users;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Clients;

public sealed class ClientAccount : ITenantOwned, IAuditable, ISoftDeletable
{
    private ClientAccount()
    {
    }

    public long Id { get; private set; }
    public long TenantId { get; private set; }
    public long ClientId { get; private set; }
    public long? StudentId { get; private set; }
    public long UserId { get; private set; }
    public User User { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public long? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long? UpdatedByUserId { get; private set; }
    public DateTimeOffset? NewsSeenAt { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public long? DeletedByUserId { get; private set; }

    public static ClientAccount Create(long clientId, User user, DateTimeOffset createdAt, long? studentId = null) =>
        new()
        {
            ClientId = clientId,
            StudentId = studentId,
            UserId = user.Id,
            User = user,
            CreatedAt = createdAt.ToUniversalTime(),
        };

    public void MarkNewsSeen(DateTimeOffset seenAt) => NewsSeenAt = seenAt.ToUniversalTime();

    public void Delete(DateTimeOffset deletedAt)
    {
        DeletedAt = SoftDeleteGuard.Delete(DeletedAt, deletedAt);
        IsDeleted = true;
    }
}
