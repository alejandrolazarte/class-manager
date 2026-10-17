using ClassManager.Core.Domain.Users;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Clients;

public sealed class ClientAccount : ITenantOwned, ISoftDeletable
{
    private ClientAccount()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid ClientId { get; private set; }
    public Guid? StudentId { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? NewsSeenAt { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public Guid? DeletedByUserId { get; private set; }

    public static ClientAccount Create(Guid clientId, User user, DateTimeOffset createdAt, Guid? studentId = null) =>
        new()
        {
            Id = Guid.CreateVersion7(),
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
