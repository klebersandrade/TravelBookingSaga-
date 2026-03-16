using TravelBooking.Common.Events;

namespace TravelBooking.Common.Commands;
public record ReserveFlightCommand: IntegrationEvent
{
    public string FlightNumber { get; init; } = string.Empty;
    public string DepartureAirport { get; init; } = string.Empty;
    public string ArrivalAirport { get; init; } = string.Empty;
    public DateTime DepartureDate { get; init; }
    public DateTime ReturnDate { get; init; }
    public int PassengerCount { get; init; }
}
