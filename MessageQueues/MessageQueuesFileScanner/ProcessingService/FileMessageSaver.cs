using System.Collections.Generic;
using MessageBroker.Kafka;

namespace ProcessingService;

public class FileMessageSaver
{
    private readonly string _path;
    private readonly Dictionary<Guid, List<FileMessageModel>> _messageCollection = new();

    public FileMessageSaver(string path)
    {
        _path = path;
    }

    public bool TrySaveMessagesInFile(FileMessageModel fileMessage)
    {
        if (_messageCollection.TryGetValue(fileMessage.FileGuid, out var fileMessageCollection))
        {
            fileMessageCollection.Add(fileMessage);
        }
        else
        {
            _messageCollection.Add(fileMessage.FileGuid, new() { fileMessage });
        }

        var allFileMessageCollection = _messageCollection[fileMessage.FileGuid];

        if (allFileMessageCollection.Count == fileMessage.Size)
        {
            IEnumerable<byte> resultBytes = Array.Empty<byte>();

            foreach (var item in allFileMessageCollection.OrderBy(x => x.Postion).Select(x => x.Payload))
            {
                resultBytes = resultBytes.Concat(item);
            }

            SaveFileMessages(resultBytes, fileMessage.FileName);
            return true;
        }

        return false;
    }

    private void SaveFileMessages(IEnumerable<byte> bytes, string fileName)
    {
        byte[] localBytes = bytes.ToArray();

        using FileStream stream = new(Path.Combine(_path, fileName), FileMode.OpenOrCreate);
        stream.Write(localBytes, 0, localBytes.Length);
    }
}
