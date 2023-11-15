namespace MessageBroker.Kafka;

public sealed class FileMessageModel
{
    public required Guid FileGuid { get; init; }
    public required string FileName { get; init; }
    public required int Size { get; init; }
    public required int Postion { get; init; }
    public required byte[] Payload { get; init; }
}
