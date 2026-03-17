using Booking.Orchestrator.Application.Commands;
using Booking.Orchestrator.Application.Responses;
using Booking.Orchestrator.Domain.Entities;
using Booking.Orchestrator.Infrastructure.Kafka;
using Booking.Orchestrator.Infrastructure.Repositories;
using TravelBooking.Common.Commands;
using TravelBooking.Common.Saga;
using TravelBooking.Infrastructure.Kafka;

namespace Booking.Orchestrator.Application.Services;

public class BookingSagaOrchestrator
{
    private readonly IBookingSagaRepository _repository;
    private readonly IKafkaProducer _kafkaProducer;

    public BookingSagaOrchestrator(IBookingSagaRepository repository, IKafkaProducer kafkaProducer)
    {
        _repository = repository;
        _kafkaProducer = kafkaProducer;
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
        await PublishNextStep(saga, command);

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

        var nextState = DetermineNextState(saga);
        saga.CurrentState = nextState;

        if (nextState == SagaState.Completed)
        {
            saga.CompletedAt = DateTime.UtcNow;
        }
        else
        {
            await PublishNextStep(saga);
        }

        await _repository.UpdateAsync(saga);
    }

    public async Task HandleStepFailed(string stepName, Guid bookingId, string reason)
    {
        var saga = await _repository.GetByIdAsync(bookingId);
        if (saga is null) return;

        saga.CurrentState = SagaState.Compensating;
        saga.FailureReason = reason;

        await PublishCompensation(saga);
        await _repository.UpdateAsync(saga);
    }

    private SagaState DetermineNextState(BookingSaga saga)
    {
        return saga.CurrentState switch
        {
            SagaState.Started when saga.HasFlight => SagaState.FlightReserving,
            SagaState.Started when saga.HasCar => SagaState.CarReserving,
            SagaState.Started when saga.HasHotel => SagaState.HotelReserving,

            SagaState.FlightReserved when saga.HasCar => SagaState.CarReserving,
            SagaState.FlightReserved when saga.HasHotel => SagaState.HotelReserving,
            SagaState.FlightReserved => SagaState.Paying,

            SagaState.CarReserved when saga.HasHotel => SagaState.HotelReserving,
            SagaState.CarReserved => SagaState.Paying,

            SagaState.HotelReserved => SagaState.Paying,

            SagaState.Paid => SagaState.Completed,

            _ => saga.CurrentState
        };
    }

    private async Task PublishNextStep(BookingSaga saga, CreateBookingCommand? command = null)
    {
        var nextState = DetermineNextState(saga);
        saga.CurrentState = nextState;

        switch (nextState)
        {
            case SagaState.FlightReserving:
                await _kafkaProducer.PublishAsync(KafkaTopics.FlightCommands, saga.Id.ToString(),
                    new ReserveFlightCommand
                    {
                        BookingId = saga.Id,
                        CorrelationId = saga.CorrelationId,
                        MessageType = nameof(ReserveFlightCommand),
                        FlightNumber = command?.FlightNumber ?? string.Empty,
                        DepartureAirport = command?.DepartureAirport ?? string.Empty,
                        ArrivalAirport = command?.ArrivalAirport ?? string.Empty,
                        DepartureDate = command?.DepartureDate ?? DateTime.UtcNow,
                        ReturnDate = command?.ReturnDate ?? DateTime.UtcNow,
                        PassengerCount = command?.PassengerCount ?? 1
                    });
                break;

            case SagaState.CarReserving:
                await _kafkaProducer.PublishAsync(KafkaTopics.CarCommands, saga.Id.ToString(),
                    new ReserveCarCommand
                    {
                        BookingId = saga.Id,
                        CorrelationId = saga.CorrelationId,
                        MessageType = nameof(ReserveCarCommand),
                        PickupLocation = command?.PickupLocation ?? string.Empty,
                        DropoffLocation = command?.DropoffLocation ?? string.Empty,
                        PickupDate = command?.PickupDate ?? DateTime.UtcNow,
                        DropoffDate = command?.DropoffDate ?? DateTime.UtcNow,
                        CarType = command?.CarType ?? string.Empty
                    });
                break;

            case SagaState.HotelReserving:
                await _kafkaProducer.PublishAsync(KafkaTopics.HotelCommands, saga.Id.ToString(),
                    new ReserveHotelCommand
                    {
                        BookingId = saga.Id,
                        CorrelationId = saga.CorrelationId,
                        MessageType = nameof(ReserveHotelCommand),
                        HotelName = command?.HotelName ?? string.Empty,
                        City = command?.City ?? string.Empty,
                        CheckInDate = command?.CheckInDate ?? DateTime.UtcNow,
                        CheckOutDate = command?.CheckOutDate ?? DateTime.UtcNow,
                        RoomCount = command?.RoomCount ?? 1,
                        RoomType = command?.RoomType ?? string.Empty
                    });
                break;

            case SagaState.Paying:
                await _kafkaProducer.PublishAsync(KafkaTopics.PaymentCommands, saga.Id.ToString(),
                    new ProcessPaymentCommand
                    {
                        BookingId = saga.Id,
                        CorrelationId = saga.CorrelationId,
                        MessageType = nameof(ProcessPaymentCommand),
                        TotalAmount = 0,
                        Currency = "BRL"
                    });
                break;
        }

        await _repository.UpdateAsync(saga);
    }

    private async Task PublishCompensation(BookingSaga saga)
    {
        var completedSteps = saga.CompletedSteps.ToList();
        completedSteps.Reverse();

        foreach (var step in completedSteps)
        {
            switch (step)
            {
                case nameof(ReserveFlightCommand):
                    await _kafkaProducer.PublishAsync(KafkaTopics.FlightCommands, saga.Id.ToString(),
                        new CancelFlightCommand
                        {
                            BookingId = saga.Id,
                            CorrelationId = saga.CorrelationId,
                            MessageType = nameof(CancelFlightCommand),
                            Reason = saga.FailureReason ?? "Compensation"
                        });
                    break;

                case nameof(ReserveCarCommand):
                    await _kafkaProducer.PublishAsync(KafkaTopics.CarCommands, saga.Id.ToString(),
                        new CancelCarCommand
                        {
                            BookingId = saga.Id,
                            CorrelationId = saga.CorrelationId,
                            MessageType = nameof(CancelCarCommand),
                            Reason = saga.FailureReason ?? "Compensation"
                        });
                    break;

                case nameof(ReserveHotelCommand):
                    await _kafkaProducer.PublishAsync(KafkaTopics.HotelCommands, saga.Id.ToString(),
                        new CancelHotelCommand
                        {
                            BookingId = saga.Id,
                            CorrelationId = saga.CorrelationId,
                            MessageType = nameof(CancelHotelCommand),
                            Reason = saga.FailureReason ?? "Compensation"
                        });
                    break;
            }
        }

        saga.CurrentState = SagaState.Failed;
    }
}
