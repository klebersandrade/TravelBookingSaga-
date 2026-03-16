using TravelBooking.Common.Events;

namespace TravelBooking.Common.Commands;
public record ReserveCarCommand: IntegrationEvent
{
    public string PickupLocation { get; init; } = string.Empty;
    public string DropoffLocation { get; init; } = string.Empty;
    public DateTime PickupDate { get; init; }
    public DateTime DropoffDate { get; init; }
    public string CarType { get; init; } = string.Empty;
}
