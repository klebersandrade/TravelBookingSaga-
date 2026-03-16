namespace Booking.Orchestrator.Application.Commands;

public record CreateBookingCommand
{
    public bool HasFlight { get; init; }
    public bool HasCar { get; init; }
    public bool HasHotel { get; init; }
    public string? FlightNumber { get; init; }
    public string? DepartureAirport { get; init; }
    public string? ArrivalAirport { get; init; }
    public DateTime? DepartureDate { get; init; }
    public DateTime? ReturnDate { get; init; }
    public int? PassengerCount { get; init; }
    public string? PickupLocation { get; init; }
    public string? DropoffLocation { get; init; }
    public DateTime? PickupDate { get; init; }
    public DateTime? DropoffDate { get; init; }
    public string? CarType { get; init; }
    public string? HotelName { get; init; }
    public string? City { get; init; }
    public DateTime? CheckInDate { get; init; }
    public DateTime? CheckOutDate { get; init; }
    public int? RoomCount { get; init; }
    public string? RoomType { get; init; }
}
