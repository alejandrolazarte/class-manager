using System.Threading.Channels;

namespace ClassManager.Infrastructure.WebPush;

public sealed class FamilyPushOutbox
{
    private readonly Channel<FamilyPush> _channel = Channel.CreateUnbounded<FamilyPush>(new UnboundedChannelOptions { SingleReader = true });

    public void Enqueue(FamilyPush push) => _channel.Writer.TryWrite(push);

    public IAsyncEnumerable<FamilyPush> ReadAllAsync(CancellationToken cancellationToken) => _channel.Reader.ReadAllAsync(cancellationToken);
}
