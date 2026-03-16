using TravelBooking.Common.Events;

namespace TravelBooking.Common.Commands;
public record ReserveHotelCommand: IntegrationEvent
{
    public string HotelName { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public DateTime CheckInDate { get; init; }
    public DateTime CheckOutDate { get; init; }
    public int RoomCount { get; init; }
    public string RoomType { get; init; } = string.Empty;
}
