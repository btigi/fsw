using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

var configPath = @"config.json";
if (!File.Exists(configPath))
{
    throw new FileNotFoundException("Configuration file not found.", configPath);
}

var configContent = File.ReadAllText(configPath);
var configurations = JsonSerializer.Deserialize<List<WatcherConfig>>(configContent) ?? throw new InvalidOperationException("Invalid configuration format.");

foreach (var config in configurations)
{
    var regex = new Regex(config.FileSpecifier, RegexOptions.IgnoreCase);

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
            await Task.Delay(config.Delay);

            if (!string.IsNullOrEmpty(e.Name) && regex.IsMatch(e.Name))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = config.ApplicationToLaunch,
                    Arguments = e.FullPath,
                    UseShellExecute = true
                });
            }
        };
    }

    if (config.NotificationTypes.Contains("Changed", StringComparer.OrdinalIgnoreCase))
    {
        fsw.Changed += async (s, e) =>
        {
            await Task.Delay(config.Delay);

            if (!string.IsNullOrEmpty(e.Name) && regex.IsMatch(e.Name))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = config.ApplicationToLaunch,
                    Arguments = e.FullPath,
                    UseShellExecute = true
                });
            }
        };
    }

    if (config.NotificationTypes.Contains("Deleted", StringComparer.OrdinalIgnoreCase))
    {
        fsw.Deleted += async (s, e) =>
        {
            await Task.Delay(config.Delay);

            if (!string.IsNullOrEmpty(e.Name) && regex.IsMatch(e.Name))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = config.ApplicationToLaunch,
                    Arguments = e.FullPath,
                    UseShellExecute = true
                });
            }
        };
    }

    if (config.NotificationTypes.Contains("Renamed", StringComparer.OrdinalIgnoreCase))
    {
        fsw.Renamed += async (s, e) =>
        {
            await Task.Delay(config.Delay);

            if (!string.IsNullOrEmpty(e.Name) && regex.IsMatch(e.Name))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = config.ApplicationToLaunch,
                    Arguments = e.FullPath,
                    UseShellExecute = true
                });
            }
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
    public List<string> NotificationTypes { get; set; } = new List<string>();
}