namespace TravelBooking.Common.Events;
public record IntegrationEvent
{
    public Guid BookingId { get; init; }
    public Guid CorrelationId { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public string MessageType { get; init; } = string.Empty;
}
