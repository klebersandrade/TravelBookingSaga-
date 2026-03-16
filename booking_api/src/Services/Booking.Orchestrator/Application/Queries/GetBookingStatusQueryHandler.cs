using Booking.Orchestrator.Application.Responses;
using Booking.Orchestrator.Infrastructure.Repositories;
using MediatR;

namespace Booking.Orchestrator.Application.Queries;

public class GetBookingStatusQueryHandler : IRequestHandler<GetBookingStatusQuery, BookingResponse?>
{
    private readonly IBookingSagaRepository _repository;

    public GetBookingStatusQueryHandler(IBookingSagaRepository repository)
    {
        _repository = repository;
    }

    public async Task<BookingResponse?> Handle(GetBookingStatusQuery request, CancellationToken cancellationToken)
    {
        var saga = await _repository.GetByIdAsync(request.BookingId);

        if (saga is null)
            return null;

        return new BookingResponse
        {
            BookingId = saga.Id,
            CorrelationId = saga.CorrelationId,
            Status = saga.CurrentState,
            Message = saga.FailureReason ?? GetStatusMessage(saga.CurrentState),
            CreatedAt = saga.CreatedAt
        };
    }

    private static string GetStatusMessage(TravelBooking.Common.Saga.SagaState state) => state switch
    {
        TravelBooking.Common.Saga.SagaState.Started => "Booking saga started",
        TravelBooking.Common.Saga.SagaState.Completed => "Booking completed successfully",
        TravelBooking.Common.Saga.SagaState.Failed => "Booking failed",
        TravelBooking.Common.Saga.SagaState.Compensating => "Booking is being compensated",
        _ => $"Booking in progress: {state}"
    };
}
