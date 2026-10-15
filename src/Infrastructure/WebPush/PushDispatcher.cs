using System.Text.Encodings.Web;
using System.Text.Json;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Infrastructure.Persistence;
using ClassManager.Notifications.WebPush;

namespace ClassManager.Infrastructure.WebPush;

public sealed class PushDispatcher
{
    private static readonly JsonSerializerOptions PayloadOptions = new(JsonSerializerDefaults.Web)
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private readonly AppDbContext _context;
    private readonly IWebPushSender _sender;
    private readonly IIdentityService _identityService;

    internal PushDispatcher(AppDbContext context, IWebPushSender sender, IIdentityService identityService)
    {
        _context = context;
        _sender = sender;
        _identityService = identityService;
    }

    public Task DispatchAsync(PushJob push, CancellationToken cancellationToken) =>
        push switch
        {
            StudentAppPush studentAppPush => DispatchToStudentsAsync(studentAppPush, cancellationToken),
            TeamPush teamPush => DispatchToMembersAsync(teamPush, cancellationToken),
            _ => Task.CompletedTask,
        };

    private async Task DispatchToStudentsAsync(StudentAppPush push, CancellationToken cancellationToken)
    {
        var query = _context.PushSubscriptions.AsQueryable();
        if (push.ClientIds is { } clientIds)
        {
            query = query.Where(subscription => clientIds.Contains(subscription.ClientId));
        }

        if (push.UserIds is { } userIds)
        {
            query = query.Where(subscription => userIds.Contains(subscription.UserId));
        }

        var subscriptions = await query.ToListAsync(cancellationToken);
        var payloads = await PayloadsByUserAsync(push, subscriptions.Select(subscription => subscription.UserId), cancellationToken);
        foreach (var subscription in subscriptions)
        {
            if (await SendAsync(subscription.Endpoint, subscription.P256dh, subscription.Auth, payloads(subscription.UserId), cancellationToken) == PushDelivery.Gone)
            {
                _context.PushSubscriptions.Remove(subscription);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task DispatchToMembersAsync(TeamPush push, CancellationToken cancellationToken)
    {
        var subscriptions = await _context.MemberPushSubscriptions
            .Where(subscription => push.UserIds.Contains(subscription.UserId))
            .ToListAsync(cancellationToken);
        var payloads = await PayloadsByUserAsync(push, subscriptions.Select(subscription => subscription.UserId), cancellationToken);
        foreach (var subscription in subscriptions)
        {
            if (await SendAsync(subscription.Endpoint, subscription.P256dh, subscription.Auth, payloads(subscription.UserId), cancellationToken) == PushDelivery.Gone)
            {
                _context.MemberPushSubscriptions.Remove(subscription);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task<Func<Guid, string>> PayloadsByUserAsync(PushJob push, IEnumerable<Guid> userIds, CancellationToken cancellationToken)
    {
        var defaultPayload = Serialize(push.Message);
        if (push.MessagesByLanguage is null)
        {
            return _ => defaultPayload;
        }

        var accounts = await _identityService.ListAccountsAsync([.. userIds.Distinct()], cancellationToken);
        var payloadsByUser = accounts.ToDictionary(account => account.UserId, account => Serialize(push.MessageIn(account.PreferredLanguage)));
        return userId => payloadsByUser.GetValueOrDefault(userId, defaultPayload);
    }

    private Task<PushDelivery> SendAsync(string endpoint, string p256dh, string auth, string payload, CancellationToken cancellationToken) =>
        _sender.SendAsync(new PushTarget(endpoint, p256dh, auth), payload, cancellationToken);

    private static string Serialize(PushMessage message) => JsonSerializer.Serialize(message, PayloadOptions);
}
