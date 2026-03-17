using System.Text.Json;
using Booking.Orchestrator.Application.Services;
using TravelBooking.Common.Events;
using TravelBooking.Infrastructure.Kafka;
using static Confluent.Kafka.ConfigPropertyNames;

namespace Booking.Orchestrator.Infrastructure.Kafka;

public class BookingSagaConsumer: BackgroundService
{
    private readonly IKafkaConsumer _consumer;
    private readonly IServiceScopeFactory _scopeFactory;

    public BookingSagaConsumer(IKafkaConsumer consumer, IServiceScopeFactory scopeFactory)
    {
        _consumer = consumer;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _consumer.StartAsync(KafkaTopics.SagaEvents, async (message) => {
            using var scope = _scopeFactory.CreateScope();
            var orchestrator = scope.ServiceProvider.GetRequiredService<BookingSagaOrchestrator>();

            var integrationEvent = JsonSerializer.Deserialize<IntegrationEvent>(message);
            if (integrationEvent == null) return;

            if (integrationEvent.MessageType.Contains("Reserved") ||
            integrationEvent.MessageType.Contains("Completed"))
            {
                await orchestrator.HandleStepCompleted(
                    integrationEvent.MessageType,
                    integrationEvent.BookingId);
            }
            else if (integrationEvent.MessageType.Contains("Failed"))
            {
                await orchestrator.HandleStepFailed(
                    integrationEvent.MessageType,
                    integrationEvent.BookingId,
                    "Step failed");
            }
        }, stoppingToken);
    }
}
