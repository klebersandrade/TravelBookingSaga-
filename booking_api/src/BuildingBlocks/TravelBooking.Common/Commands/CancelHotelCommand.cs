using TravelBooking.Common.Events;

namespace TravelBooking.Common.Commands;
public record CancelHotelCommand: IntegrationEvent
{
    public string Reason { get; init; } = string.Empty;
}
