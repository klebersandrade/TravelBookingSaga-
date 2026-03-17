namespace Booking.Orchestrator.Infrastructure.Kafka;

public static class KafkaTopics
{
    public const string FlightCommands = "flight-commands";
    public const string CarCommands = "car-commands";
    public const string HotelCommands = "hotel-commands";
    public const string PaymentCommands = "payment-commands";
    public const string SagaEvents = "saga-events";
}
