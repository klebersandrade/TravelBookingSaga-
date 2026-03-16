namespace TravelBooking.Common.Events;
public record BookingFailedEvent: IntegrationEvent
{
    public string Reason { get; init; } = string.Empty;
    public string FailedAtStep { get; init; } = string.Empty;
}
