namespace DataCaptureService;

public sealed class FolderListener : IDisposable
{
    public string Path { get; init; }
    public string FileType { get; init; }

    private FileSystemWatcher Watcher { get; set; }

    public FolderListener(string path, string fileType = "pdf")
    {
        Path = path;
        FileType = fileType;

        Watcher = new(Path, $"*.{FileType}")
        {
            IncludeSubdirectories = true,
            EnableRaisingEvents = true,
            NotifyFilter = NotifyFilters.Attributes
                         | NotifyFilters.CreationTime
                         | NotifyFilters.DirectoryName
                         | NotifyFilters.FileName
                         | NotifyFilters.LastAccess
                         | NotifyFilters.LastWrite
                         | NotifyFilters.Security
                         | NotifyFilters.Size,
        };
    }

    public event FileSystemEventHandler? Created
    {
        add
        {
            Watcher.Created += value;
        }
        remove
        {
            Watcher.Created -= value;
        }
    }

    public void Dispose()
    {
        Watcher?.Dispose();
        GC.SuppressFinalize(this);
    }
}
