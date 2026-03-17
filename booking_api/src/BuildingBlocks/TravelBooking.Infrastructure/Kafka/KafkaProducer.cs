using System.Text.Json;
using Confluent.Kafka;
using TravelBooking.Common.Events;

namespace TravelBooking.Infrastructure.Kafka;

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<string, string> _producer;

    public KafkaProducer(IProducer<string, string> producer)
    {
        _producer = producer;
    }

    public async Task PublishAsync<T>(string topic, string key, T message) where T : IntegrationEvent
    {
        var json = JsonSerializer.Serialize(message);
        var msg = new Message<string, string> { Key = key, Value = json };
        await _producer.ProduceAsync(topic, msg);
    }
}
