using Booking.Orchestrator.Application.Responses;
using Booking.Orchestrator.Application.Services;
using MediatR;

namespace Booking.Orchestrator.Application.Commands;

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, BookingResponse>
{
    private readonly BookingSagaOrchestrator _orchestrator;

    public CreateBookingCommandHandler(BookingSagaOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }

    public async Task<BookingResponse> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        return await _orchestrator.StartSaga(request);
    }
}
