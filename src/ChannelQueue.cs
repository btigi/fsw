using System.Threading.Channels;

namespace fsw
{
    public class ChannelQueue
    {
        private readonly Channel<FileSystemEventArgs> _channel;

        public ChannelQueue(int queueSize = 100)
        {
            _channel = Channel.CreateBounded<FileSystemEventArgs>(new BoundedChannelOptions(queueSize)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true
            });
        }

        public async Task Produce(FileSystemEventArgs @event)
        {
            await _channel.Writer.WriteAsync(@event);
        }

        public async ValueTask<FileSystemEventArgs> Consume(CancellationToken cancellationToken)
        {
            return await _channel.Reader.ReadAsync(cancellationToken);
        }
    }
}