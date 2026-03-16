namespace TravelBooking.Common.Events;
public record CarReservedEvent: IntegrationEvent
{
    public string ReservationCode { get; init; } = string.Empty;
    public string CarType { get; init; } = string.Empty;
    public decimal Price { get; init; }
}
