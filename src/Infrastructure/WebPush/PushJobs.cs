namespace ClassManager.Infrastructure.WebPush;

public sealed record PushMessage(string Title, string Body, string Url);

public abstract record PushJob(Guid TenantId, PushMessage Message);

public sealed record FamilyPush(Guid TenantId, IReadOnlyCollection<Guid>? ClientIds, PushMessage Message) : PushJob(TenantId, Message);

public sealed record TeamPush(Guid TenantId, IReadOnlyCollection<Guid> UserIds, PushMessage Message) : PushJob(TenantId, Message);
