using Confluent.Kafka;
using System.Text;
using System.Text.Json;

namespace MessageBroker.Kafka;

public class MessageModelSerializer<T> : ISerializer<T> where T : class 
{
    public byte[] Serialize(T data, SerializationContext context)
    {
        return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));
    }
}
