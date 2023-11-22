using MessageBroker.Kafka;
using ProcessingService;

#region Get start data
Console.WriteLine("Write folder path to save files or use ENTER for default settings:");

var folderInput = Console.ReadLine()?.Trim();
var pathToSave = string.IsNullOrWhiteSpace(folderInput) ? string.Empty : folderInput;
#endregion

#region Init functionality
using CancellationTokenSource source = new();
using MessageBus<FileMessageModel> messageBus = new();
FileMessageSaver saver = new(pathToSave);

_ = Task.Run(() => messageBus.SubscribeOnTopic<FileMessageModel>("file-topic", OnReadMessage, source.Token));

void OnReadMessage(FileMessageModel? fileMessage)
{
    if (fileMessage is null)
    {
        return;
    }

    var isSaved = saver.TrySaveMessagesInFile(fileMessage!);

    if (isSaved) 
    {
        Console.WriteLine($"File: {fileMessage.FileName} was seved");
    }
}
#endregion

#region Work with CommandLineInterface
Console.WriteLine("Use 'q' key to exit...");

string input = string.Empty;

for (;input.Trim().ToUpper() != "Q";)
{
    input = Console.ReadLine() ?? string.Empty;
}

source.Cancel();

Console.WriteLine("Press any key to continue");
Console.ReadLine();
#endregion