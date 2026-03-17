using Confluent.Kafka;

namespace TravelBooking.Infrastructure.Kafka;

public class KafkaConsumer : IKafkaConsumer
{
    private readonly KafkaSettings _settings;
    public KafkaConsumer(KafkaSettings settings)
    {
        _settings = settings;
    }
    public async Task StartAsync(string topic, Func<string, Task> onMessage, CancellationToken cancellationToken)
    {
        var consumeConfig = new ConsumerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            GroupId = _settings.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };

        using var consumer = new ConsumerBuilder<string, string>(consumeConfig).Build();
        consumer.Subscribe(topic);

        while (!cancellationToken.IsCancellationRequested)
        {
            var result = consumer.Consume(cancellationToken);

            if (result?.Message?.Value != null)
            {
                await onMessage(result.Message.Value);
            } 
        }
    }
}
