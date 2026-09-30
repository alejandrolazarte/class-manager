namespace ClassManager.Infrastructure.WebPush;

public sealed record FamilyPushMessage(string Title, string Body, string Url);

public sealed record FamilyPush(Guid TenantId, IReadOnlyCollection<Guid>? ClientIds, FamilyPushMessage Message);
