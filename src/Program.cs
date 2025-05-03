using fsw;
using System.Text.Json;

var configPath = @"config.json";
if (!File.Exists(configPath))
{
    throw new FileNotFoundException("Configuration file not found.", configPath);
}

var configContent = File.ReadAllText(configPath);
var configurations = JsonSerializer.Deserialize<List<WatcherConfig>>(configContent) ?? throw new InvalidOperationException("Invalid configuration format.");

var tasks = new List<Task>();

foreach (var config in configurations)
{
    var cts = new CancellationTokenSource();
    var channelQueue = new ChannelQueue(config.QueueSize);
    var consumer = new Consumer(channelQueue, config);
    tasks.Add(consumer.ExecuteAsync(cts.Token));

    var fsw = new FileSystemWatcher();
    var directoryPath = Path.GetDirectoryName(config.Path);
    if (string.IsNullOrEmpty(directoryPath))
    {
        throw new InvalidOperationException($"The specified path '{config.Path}' does not contain valid directory information.");
    }

    fsw.Path = directoryPath;
    fsw.IncludeSubdirectories = config.IncludeSubdirectories;

    if (config.NotificationTypes.Contains("Created", StringComparer.OrdinalIgnoreCase))
    {
        fsw.Created += async (s, e) =>
        {
            await channelQueue.Produce(e);
        };
    }

    if (config.NotificationTypes.Contains("Changed", StringComparer.OrdinalIgnoreCase))
    {
        fsw.Changed += async (s, e) =>
        {
            await channelQueue.Produce(e);
        };
    }

    if (config.NotificationTypes.Contains("Deleted", StringComparer.OrdinalIgnoreCase))
    {
        fsw.Deleted += async (s, e) =>
        {
            await channelQueue.Produce(e);
        };
    }

    if (config.NotificationTypes.Contains("Renamed", StringComparer.OrdinalIgnoreCase))
    {
        fsw.Renamed += async (s, e) =>
        {
            await channelQueue.Produce(e);
        };
    }

    fsw.EnableRaisingEvents = true;
}

Console.WriteLine("File watchers are running. Press Enter to exit.");
Console.ReadLine();

public class WatcherConfig
{
    public string Path { get; set; } = string.Empty;
    public string FileSpecifier { get; set; } = string.Empty;
    public string ApplicationToLaunch { get; set; } = string.Empty;
    public bool IncludeSubdirectories { get; set; } = false;
    public int Delay { get; set; } = 500;
    public int QueueSize { get; set; } = 100;
    public List<string> NotificationTypes { get; set; } = new List<string>();
}