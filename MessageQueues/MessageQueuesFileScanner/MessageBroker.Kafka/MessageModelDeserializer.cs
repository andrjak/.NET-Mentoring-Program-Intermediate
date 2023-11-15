using Confluent.Kafka;
using System.Text;
using System.Text.Json;

namespace MessageBroker.Kafka;

public class MessageModelDeserializer<T> : IDeserializer<T?> where T : class
{
    public T? Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull)
        {
            return null;
        }

        var jsonString = Encoding.UTF8.GetString(data);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<T?>(jsonString, options);
    }
}
