using System.Text.Json.Serialization;
using ClassManager.Notifications.WebPush;

namespace ClassManager.Infrastructure.WebPush;

[JsonDerivedType(typeof(StudentAppPush), StudentAppPush.Discriminator)]
[JsonDerivedType(typeof(TeamPush), TeamPush.Discriminator)]
public abstract record PushJob(Guid TenantId, PushMessage Message);

public sealed record StudentAppPush(Guid TenantId, IReadOnlyCollection<Guid>? ClientIds, PushMessage Message, IReadOnlyCollection<Guid>? UserIds = null)
    : PushJob(TenantId, Message)
{
    public const string Discriminator = "students";
}

public sealed record TeamPush(Guid TenantId, IReadOnlyCollection<Guid> UserIds, PushMessage Message) : PushJob(TenantId, Message)
{
    public const string Discriminator = "team";
}
