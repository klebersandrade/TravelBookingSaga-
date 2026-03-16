namespace TravelBooking.Common.Saga;

public enum SagaState
{
    Started,
    FlightReserving,
    FlightReserved,
    CarReserving,
    CarReserved,
    HotelReserving,
    HotelReserved,
    Paying,
    Paid,
    Confirming,
    Completed,
    Compensating,
    Failed
}
