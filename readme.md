fsw
=========

File System Watcher is a C# console application to trigger a command when on file changes.

## Configuration

The application is configured through a json file names `config.json`. The json file contains an array of WatcherConfig entries. Each entry consists of:

- Path - the path to monitor
- File specifier - a regex to indicate the files to monitor
- Application to launch - the full path of the application to launch. The application receives the full path to the file that triggers the monitor as the first argument
- Include subdirectories - whether to include subdirectories
- Delay - millisends to wait after the monitor is triggered before calling the application
- Notification Types - array of monitoring events, possible values are: Created, Changed, Deleted, Renamed


An example `config.json` file is below:

```
[  
 {  
   "Path": "D:\\data\\",  
   "FileSpecifier": "^.*\\.txt$",  
   "ApplicationToLaunch": "notepad.exe",  
   "IncludeSubdirectories": false,  
   "Delay": 2500,  
   "NotificationTypes": ["Created", "Changed", "Deleted", "Renamed"]  
 },  
 {  
   "Path": "D:\\data\\a",  
   "FileSpecifier": "^.*\\.log$",  
   "ApplicationToLaunch": "notepad.exe",  
   "IncludeSubdirectories": false,  
   "Delay": 500,  
   "NotificationTypes": ["Created", "Changed", "Deleted", "Renamed"]  
 }  
]
```

## Download

Compiled downloads are not available.

## Compiling

To clone and run this application, you'll need [Git](https://git-scm.com) and [.NET](https://dotnet.microsoft.com/) installed on your computer. From your command line:

```
# Clone this repository
$ git clone https://github.com/btigi/fsw

# Go into the repository
$ cd src

# Build  the app
$ dotnet build
```

## Licencing

fsw is licenced under the MIT License. Full licence details are available in licence.md