using System.Threading.Channels;

namespace ClassManager.Infrastructure.WebPush;

public sealed class PushOutbox
{
    private readonly Channel<PushJob> _channel = Channel.CreateUnbounded<PushJob>(new UnboundedChannelOptions { SingleReader = true });

    public void Enqueue(PushJob push) => _channel.Writer.TryWrite(push);

    public IAsyncEnumerable<PushJob> ReadAllAsync(CancellationToken cancellationToken) => _channel.Reader.ReadAllAsync(cancellationToken);
}
