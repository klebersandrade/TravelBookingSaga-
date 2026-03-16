namespace TravelBooking.Common.Events;
public record FlightReservedEvent: IntegrationEvent
{
    public string FlightNumber { get; init; } = string.Empty;
    public string ReservationCode { get; init; } = string.Empty;
    public decimal Price { get; init; }
}
