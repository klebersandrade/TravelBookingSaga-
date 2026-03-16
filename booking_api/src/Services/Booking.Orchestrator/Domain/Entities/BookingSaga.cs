using TravelBooking.Common.Saga;

namespace Booking.Orchestrator.Domain.Entities;

public class BookingSaga
{
    public Guid Id { get; set; }
    public Guid CorrelationId { get; set; }
    public SagaState CurrentState { get; set; }
    public bool HasFlight { get; set; }
    public bool HasCar { get; set; }
    public bool HasHotel { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? FailureReason { get; set; }
    public List<string> CompletedSteps { get; set; } = [];

}
