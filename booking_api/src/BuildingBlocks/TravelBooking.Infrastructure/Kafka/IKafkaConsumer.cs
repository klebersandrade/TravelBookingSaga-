namespace TravelBooking.Infrastructure.Kafka;

public interface IKafkaConsumer
{
    Task StartAsync(string topic, Func<string, Task> onMessage, CancellationToken cancellationToken);
}
