using System.Text.Encodings.Web;
using System.Text.Json;
using ClassManager.Infrastructure.Persistence;

namespace ClassManager.Infrastructure.WebPush;

public sealed class FamilyPushDispatcher
{
    private static readonly JsonSerializerOptions PayloadOptions = new(JsonSerializerDefaults.Web)
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private readonly AppDbContext _context;
    private readonly IWebPushSender _sender;

    internal FamilyPushDispatcher(AppDbContext context, IWebPushSender sender)
    {
        _context = context;
        _sender = sender;
    }

    public async Task DispatchAsync(FamilyPush push, CancellationToken cancellationToken)
    {
        var query = _context.PushSubscriptions.AsQueryable();
        if (push.ClientIds is { } clientIds)
        {
            query = query.Where(subscription => clientIds.Contains(subscription.ClientId));
        }

        var subscriptions = await query.ToListAsync(cancellationToken);
        var payload = JsonSerializer.Serialize(push.Message, PayloadOptions);
        foreach (var subscription in subscriptions)
        {
            var delivery = await _sender.SendAsync(
                new PushTarget(subscription.Endpoint, subscription.P256dh, subscription.Auth), payload, cancellationToken);
            if (delivery == PushDelivery.Gone)
            {
                _context.PushSubscriptions.Remove(subscription);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
