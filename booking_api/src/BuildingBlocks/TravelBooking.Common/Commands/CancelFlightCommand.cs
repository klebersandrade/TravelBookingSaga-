using TravelBooking.Common.Events;

namespace TravelBooking.Common.Commands;
public record CancelFlightCommand: IntegrationEvent
{
    public string Reason { get; init; } = string.Empty;
}
