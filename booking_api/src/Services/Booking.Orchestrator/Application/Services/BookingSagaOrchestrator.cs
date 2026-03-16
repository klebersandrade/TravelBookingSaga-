using Booking.Orchestrator.Application.Commands;
using Booking.Orchestrator.Application.Responses;
using Booking.Orchestrator.Domain.Entities;
using Booking.Orchestrator.Infrastructure.Repositories;
using TravelBooking.Common.Saga;

namespace Booking.Orchestrator.Application.Services;

public class BookingSagaOrchestrator
{
    private readonly IBookingSagaRepository _repository;

    public BookingSagaOrchestrator(IBookingSagaRepository repository)
    {
        _repository = repository;
    }

    public async Task<BookingResponse> StartSaga(CreateBookingCommand command)
    {
        var saga = new BookingSaga
        {
            Id = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            CurrentState = SagaState.Started,
            HasFlight = command.HasFlight,
            HasCar = command.HasCar,
            HasHotel = command.HasHotel,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.CreateAsync(saga);

        // TODO: Publish first command to Kafka

        return new BookingResponse
        {
            BookingId = saga.Id,
            CorrelationId = saga.CorrelationId,
            Status = saga.CurrentState,
            Message = "Booking saga started",
            CreatedAt = saga.CreatedAt
        };
    }

    public async Task HandleStepCompleted(string stepName, Guid bookingId)
    {
        var saga = await _repository.GetByIdAsync(bookingId);
        if (saga is null) return;

        saga.CompletedSteps.Add(stepName);

        // TODO: Determine next state and publish next command to Kafka

        await _repository.UpdateAsync(saga);
    }

    public async Task HandleStepFailed(string stepName, Guid bookingId, string reason)
    {
        var saga = await _repository.GetByIdAsync(bookingId);
        if (saga is null) return;

        saga.CurrentState = SagaState.Compensating;
        saga.FailureReason = reason;

        // TODO: Publish compensation commands to Kafka (reverse order)

        await _repository.UpdateAsync(saga);
    }
}
