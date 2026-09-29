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
    public Guid UserId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public static ClientAccount Create(Guid clientId, Guid userId, DateTimeOffset createdAt) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            ClientId = clientId,
            UserId = userId,
            CreatedAt = createdAt.ToUniversalTime(),
        };
}
