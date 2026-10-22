using System.Text.Json.Serialization;
using ClassManager.Notifications.WebPush;

namespace ClassManager.Infrastructure.WebPush;

[JsonDerivedType(typeof(StudentAppPush), StudentAppPush.Discriminator)]
[JsonDerivedType(typeof(TeamPush), TeamPush.Discriminator)]
public abstract record PushJob(long TenantId, PushMessage Message)
{
    public IReadOnlyDictionary<string, PushMessage>? MessagesByLanguage { get; init; }

    public PushMessage MessageIn(string? language) =>
        language is not null && MessagesByLanguage?.TryGetValue(language, out var message) == true ? message : Message;
}

public sealed record StudentAppPush(long TenantId, IReadOnlyCollection<long>? ClientIds, PushMessage Message, IReadOnlyCollection<Guid>? UserIds = null)
    : PushJob(TenantId, Message)
{
    public const string Discriminator = "students";
}

public sealed record TeamPush(long TenantId, IReadOnlyCollection<Guid> UserIds, PushMessage Message) : PushJob(TenantId, Message)
{
    public const string Discriminator = "team";
}
