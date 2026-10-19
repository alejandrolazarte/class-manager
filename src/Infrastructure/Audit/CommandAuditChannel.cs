using System.Threading.Channels;

namespace ClassManager.Infrastructure.Audit;

public sealed class CommandAuditChannel
{
    public const int Capacity = 10_000;

    private readonly Channel<CommandAuditEntry> _channel = Channel.CreateBounded<CommandAuditEntry>(
        new BoundedChannelOptions(Capacity)
        {
            FullMode = BoundedChannelFullMode.DropWrite,
            SingleReader = true,
        });

    public ChannelReader<CommandAuditEntry> Reader => _channel.Reader;

    public bool TryWrite(CommandAuditEntry entry) => _channel.Writer.TryWrite(entry);
}
