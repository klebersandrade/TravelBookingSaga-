using TravelBooking.Common.Events;

namespace TravelBooking.Common.Commands;
public record CancelCarCommand: IntegrationEvent
{
    public string Reason { get; init; } = string.Empty;
}
