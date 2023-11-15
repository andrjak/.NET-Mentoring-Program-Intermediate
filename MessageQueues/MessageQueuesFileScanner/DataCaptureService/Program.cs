using DataCaptureService;
using MessageBroker.Kafka;

#region Get start data
Console.WriteLine("Write folder path to listen adding new files or use ENTER for default settings:");

var folderInput = Console.ReadLine()?.Trim();
var pathToListn = string.IsNullOrWhiteSpace(folderInput) ? @"C:\TestFolder" : folderInput;

Console.WriteLine("Enter the file type to listen or use ENTER for default settings:");

var fileTypeInput = Console.ReadLine()?.Trim();
var fileTypeToListn = string.IsNullOrWhiteSpace(fileTypeInput) ? @"pdf" : fileTypeInput;
#endregion

#region Init functionality
var listner = new FolderListener(pathToListn!, fileTypeToListn!);
using MessageBus<FileMessageModel> messageBus = new();
listner.Created += OnCreated;

void OnCreated(object sender, FileSystemEventArgs e)
{
    string value = $"Created: {e.FullPath}";

    var messages = FileMessageProvider.GetFileMessages(e.FullPath, e.Name ?? string.Empty);

    foreach (var message in messages)
    {
        _ = messageBus.SendMessage("file-topic", message);
    }

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