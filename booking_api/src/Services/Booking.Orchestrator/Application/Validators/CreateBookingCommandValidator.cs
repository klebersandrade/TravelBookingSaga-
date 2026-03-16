using Booking.Orchestrator.Application.Commands;
using FluentValidation;

namespace Booking.Orchestrator.Application.Validators;

public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(x => x)
            .Must(x => x.HasFlight || x.HasCar || x.HasHotel)
            .WithMessage("At least one service (flight, car, or hotel) must be selected.");

        When(x => x.HasFlight, () =>
        {
            RuleFor(x => x.FlightNumber).NotEmpty().WithMessage("Flight number is required.");
            RuleFor(x => x.DepartureAirport).NotEmpty().WithMessage("Departure airport is required.");
            RuleFor(x => x.ArrivalAirport).NotEmpty().WithMessage("Arrival airport is required.");
            RuleFor(x => x.DepartureDate).NotNull().WithMessage("Departure date is required.");
            RuleFor(x => x.ReturnDate).NotNull().WithMessage("Return date is required.");
            RuleFor(x => x.PassengerCount).NotNull().GreaterThan(0).WithMessage("Passenger count must be greater than 0.");
            RuleFor(x => x.ReturnDate)
                .GreaterThan(x => x.DepartureDate)
                .When(x => x.DepartureDate.HasValue && x.ReturnDate.HasValue)
                .WithMessage("Return date must be after departure date.");
        });

        When(x => x.HasCar, () =>
        {
            RuleFor(x => x.PickupLocation).NotEmpty().WithMessage("Pickup location is required.");
            RuleFor(x => x.DropoffLocation).NotEmpty().WithMessage("Dropoff location is required.");
            RuleFor(x => x.PickupDate).NotNull().WithMessage("Pickup date is required.");
            RuleFor(x => x.DropoffDate).NotNull().WithMessage("Dropoff date is required.");
            RuleFor(x => x.CarType).NotEmpty().WithMessage("Car type is required.");
            RuleFor(x => x.DropoffDate)
                .GreaterThan(x => x.PickupDate)
                .When(x => x.PickupDate.HasValue && x.DropoffDate.HasValue)
                .WithMessage("Dropoff date must be after pickup date.");
        });

        When(x => x.HasHotel, () =>
        {
            RuleFor(x => x.HotelName).NotEmpty().WithMessage("Hotel name is required.");
            RuleFor(x => x.City).NotEmpty().WithMessage("City is required.");
            RuleFor(x => x.CheckInDate).NotNull().WithMessage("Check-in date is required.");
            RuleFor(x => x.CheckOutDate).NotNull().WithMessage("Check-out date is required.");
            RuleFor(x => x.RoomCount).NotNull().GreaterThan(0).WithMessage("Room count must be greater than 0.");
            RuleFor(x => x.RoomType).NotEmpty().WithMessage("Room type is required.");
            RuleFor(x => x.CheckOutDate)
                .GreaterThan(x => x.CheckInDate)
                .When(x => x.CheckInDate.HasValue && x.CheckOutDate.HasValue)
                .WithMessage("Check-out date must be after check-in date.");
        });
    }
}
