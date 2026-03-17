using TravelBooking.Common.Events;

namespace TravelBooking.Infrastructure.Kafka;

public interface IKafkaProducer
{
    Task PublishAsync<T>(string topic, string key, T message) where T : IntegrationEvent;
}
