namespace TravelBooking.Infrastructure.Kafka;

public class KafkaSettings
{
    public required string BootstrapServers { get; set; }
    public required string GroupId { get; set; }
}
