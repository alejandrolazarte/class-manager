using System.Threading.Channels;

namespace ClassManager.Infrastructure.Audit;

public sealed class AuditLogChannel<TEntry>
{
    public const int Capacity = 10_000;

    private readonly Channel<TEntry> _channel = Channel.CreateBounded<TEntry>(
        new BoundedChannelOptions(Capacity)
        {
            FullMode = BoundedChannelFullMode.DropWrite,
            SingleReader = true,
        });

    public ChannelReader<TEntry> Reader => _channel.Reader;

    public bool TryWrite(TEntry entry) => _channel.Writer.TryWrite(entry);
}
