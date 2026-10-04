using ClassManager.Notifications.WebPush;

namespace ClassManager.Infrastructure.WebPush;

public abstract record PushJob(Guid TenantId, PushMessage Message);

public sealed record StudentAppPush(Guid TenantId, IReadOnlyCollection<Guid>? ClientIds, PushMessage Message) : PushJob(TenantId, Message);

public sealed record TeamPush(Guid TenantId, IReadOnlyCollection<Guid> UserIds, PushMessage Message) : PushJob(TenantId, Message);
