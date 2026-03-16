namespace TravelBooking.Common.Events;
public record BookingCompletedEvent: IntegrationEvent
{
    public decimal TotalPrice { get; init; }
    public DateTime CompletedAt { get; init; }
}
