namespace TravelBooking.Common.Events;
public record HotelReservedEvent: IntegrationEvent
{
    public string ReservationCode { get; init; } = string.Empty;
    public string HotelName { get; init; } = string.Empty;
    public decimal Price { get; init; }
}
