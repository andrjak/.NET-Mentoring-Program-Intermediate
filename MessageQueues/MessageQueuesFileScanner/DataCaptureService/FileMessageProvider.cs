using System.Linq;
using MessageBroker.Kafka;

namespace DataCaptureService;

public static class FileMessageProvider
{
    private const int MaxSizePayload = 524288; // byte, 0.5 Mb

    public static IEnumerable<FileMessageModel> GetFileMessages(string filePath, string name)
    {
        var fileGuid = Guid.NewGuid();
        byte[] bytes = File.ReadAllBytes(filePath);

        var chanks = bytes.Chunk(MaxSizePayload).ToArray();
        int position = 0;

        return chanks.Select(fileChunk => new FileMessageModel
        {
            FileGuid = fileGuid,
            FileName = name,
            Payload = fileChunk,
            Size = chanks.Length,
            Postion = position++,
        });
    }
}
