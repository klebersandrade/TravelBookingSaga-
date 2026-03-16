using TravelBooking.Common.Saga;

namespace Booking.Orchestrator.Application.Responses;

public record BookingResponse
{
    public Guid BookingId { get; init; }
    public Guid CorrelationId { get; init; }
    public SagaState Status { get; init; }
    public string Message { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
