using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Clients;

public sealed class ClientAccount : ITenantOwned
{
    private ClientAccount()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid ClientId { get; private set; }
    public Guid? StudentId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? NewsSeenAt { get; private set; }

    public static ClientAccount Create(Guid clientId, Guid userId, DateTimeOffset createdAt, Guid? studentId = null) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            ClientId = clientId,
            StudentId = studentId,
            UserId = userId,
            CreatedAt = createdAt.ToUniversalTime(),
        };

    public void MarkNewsSeen(DateTimeOffset seenAt) => NewsSeenAt = seenAt.ToUniversalTime();
}
