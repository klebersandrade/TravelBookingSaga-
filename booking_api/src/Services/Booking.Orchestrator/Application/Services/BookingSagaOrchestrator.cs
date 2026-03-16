using Booking.Orchestrator.Application.Commands;
using Booking.Orchestrator.Application.Responses;

namespace Booking.Orchestrator.Application.Services;

public class BookingSagaOrchestrator
{
    public BookingResponse StartSaga(CreateBookingCommand command)
    {
        // 1. Create a new BookingSaga entity
        // 2. Set state to SagaState.Started
        // 3. Determine first step (Flight? Car? Hotel?)
        // 4. Publish the first command to Kafka
        // 5. Return BookingResponse
        throw new NotImplementedException();
    }

    public void HandleStepCompleted(string stepName, Guid bookingId)
    {
        // 1. Update saga state
        // 2. Add step to CompletedSteps
        // 3. Determine and publish next step
        throw new NotImplementedException();
    }

    public void HandleStepFailed(string stepName, Guid bookingId, string reason)
    {
        // 1. Set state to Compensating
        // 2. Start compensation (cancel completed steps in reverse)
        throw new NotImplementedException();
    }
}
