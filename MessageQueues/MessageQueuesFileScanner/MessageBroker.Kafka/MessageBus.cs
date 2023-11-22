using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace MessageBroker.Kafka;

public sealed class MessageBus<TMessage> : IDisposable 
    where TMessage : class 
{
    private readonly string _host;
    private readonly ProducerConfig _producerConfig;
    private readonly ConsumerConfig _consumerConfig;
    private readonly IProducer<Null, TMessage> _producer;
    private readonly IConsumer<Null, TMessage> _consumer;

    public MessageBus() : this("localhost") { }

    public MessageBus(string host)
    {
        _host = host;
        _producerConfig = new ProducerConfig
        {
            BootstrapServers = host,
            AllowAutoCreateTopics = true,
        };

        _consumerConfig = new ConsumerConfig
        {
            GroupId = "custom-group",
            BootstrapServers = host,
            AutoOffsetReset = AutoOffsetReset.Latest,
            EnableAutoCommit = true,
            AllowAutoCreateTopics = true,
            PartitionAssignmentStrategy = PartitionAssignmentStrategy.Range,
        };

        _producer = new ProducerBuilder<Null, TMessage>(_producerConfig).SetValueSerializer(new MessageModelSerializer<TMessage>()).Build();
        _consumer = new ConsumerBuilder<Null, TMessage>(_consumerConfig).SetValueDeserializer(new MessageModelDeserializer<TMessage>()!).Build();
    }

    public Task CreateTopicManually(string topicName)
    {
        var config = new AdminClientConfig
        {
            BootstrapServers = _host,
        };

        using var adminClient = new AdminClientBuilder(config).Build();

        var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(5));
        var topic = metadata.Topics.Find(t => t.Topic == topicName);

        if (topic is not null)
        {
            return Task.CompletedTask;
        }

        var numPartitions = 1;
        short replicationFactor = 1;

        var topicSpecification = new TopicSpecification
        {
            Name = topicName,
            NumPartitions = numPartitions,
            ReplicationFactor = replicationFactor
        };

        var topicSpecifications = new List<TopicSpecification> { topicSpecification };

        return adminClient.CreateTopicsAsync(topicSpecifications);
    }

    public async Task<DeliveryResult<Null, TMessage>> SendMessage(string topic, TMessage message)
    {
        return await _producer.ProduceAsync(
            topic,
            new Message<Null, TMessage>
            {
                Value = message,
            });
    }

    public async void SubscribeOnTopic<T>(string topic, Action<T?> action, CancellationToken cancellationToken) where T : class
    {
        await CreateTopicManually(topic);

        _consumer.Subscribe(topic);

        for (;;)
        {
            var consumeResult = _consumer.Consume(cancellationToken);

            if (cancellationToken.IsCancellationRequested)
            {
                _consumer.Close();
                return;
            }

            action?.Invoke(consumeResult?.Message?.Value as T);
        }
    }

    public void Dispose()
    {
        _producer?.Dispose();
        _consumer?.Dispose();
    }
}