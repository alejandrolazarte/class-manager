using System.Text.Encodings.Web;
using System.Text.Json;
using ClassManager.Infrastructure.Persistence;

namespace ClassManager.Infrastructure.WebPush;

public sealed class PushDispatcher
{
    private static readonly JsonSerializerOptions PayloadOptions = new(JsonSerializerDefaults.Web)
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private readonly AppDbContext _context;
    private readonly IWebPushSender _sender;

    internal PushDispatcher(AppDbContext context, IWebPushSender sender)
    {
        _context = context;
        _sender = sender;
    }

    public Task DispatchAsync(PushJob push, CancellationToken cancellationToken) =>
        push switch
        {
            FamilyPush familyPush => DispatchToFamiliesAsync(familyPush, cancellationToken),
            TeamPush teamPush => DispatchToMembersAsync(teamPush, cancellationToken),
            _ => Task.CompletedTask,
        };

    private async Task DispatchToFamiliesAsync(FamilyPush push, CancellationToken cancellationToken)
    {
        var query = _context.PushSubscriptions.AsQueryable();
        if (push.ClientIds is { } clientIds)
        {
            query = query.Where(subscription => clientIds.Contains(subscription.ClientId));
        }

        var payload = Serialize(push.Message);
        foreach (var subscription in await query.ToListAsync(cancellationToken))
        {
            if (await SendAsync(subscription.Endpoint, subscription.P256dh, subscription.Auth, payload, cancellationToken) == PushDelivery.Gone)
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
        var payload = Serialize(push.Message);
        foreach (var subscription in subscriptions)
        {
            if (await SendAsync(subscription.Endpoint, subscription.P256dh, subscription.Auth, payload, cancellationToken) == PushDelivery.Gone)
            {
                _context.MemberPushSubscriptions.Remove(subscription);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    private Task<PushDelivery> SendAsync(string endpoint, string p256dh, string auth, string payload, CancellationToken cancellationToken) =>
        _sender.SendAsync(new PushTarget(endpoint, p256dh, auth), payload, cancellationToken);

    private static string Serialize(PushMessage message) => JsonSerializer.Serialize(message, PayloadOptions);
}
