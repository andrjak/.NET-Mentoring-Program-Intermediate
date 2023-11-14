using DataCaptureService;

#region Init functionality
var listner = new FolderListener(@"D:\TestFolder");
listner.Created += OnCreated;

static void OnCreated(object sender, FileSystemEventArgs e)
{
    string value = $"Created: {e.FullPath}";
    Console.WriteLine(value);
}
#endregion


#region Work with CommandLineInterface
Console.WriteLine("Use 'q' key to exit...");

string input = string.Empty;

for (;input.Trim().ToUpper() != "Q";)
{
    input = Console.ReadLine() ?? string.Empty;
}

Console.WriteLine("Press any key to continue");
Console.ReadLine();
#endregion