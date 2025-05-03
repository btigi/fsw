using System.Diagnostics;
using System.Text.RegularExpressions;

namespace fsw
{
    public class Consumer
    {
        private readonly ChannelQueue _channelQueue;
        private readonly WatcherConfig _config;
        private readonly Regex _regex;

        public Consumer(ChannelQueue channelQueue, WatcherConfig config)
        {
            _channelQueue = channelQueue;
            _config = config;
            _regex = new Regex(config.FileSpecifier, RegexOptions.IgnoreCase);
        }

        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var @event = await _channelQueue.Consume(stoppingToken);
                    await ProcessEvent(@event, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing event: {ex.Message}");
                }
            }
        }

        private async Task ProcessEvent(FileSystemEventArgs @event, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(@event.Name) || !_regex.IsMatch(@event.Name))
            {
                return;
            }

            await Task.Delay(_config.Delay, cancellationToken);

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = _config.ApplicationToLaunch,
                    Arguments = @event.FullPath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error launching application: {ex.Message}");
            }
        }
    }
}