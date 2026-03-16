using Booking.Orchestrator.Application.Responses;
using MediatR;

namespace Booking.Orchestrator.Application.Queries;

public record GetBookingStatusQuery(Guid BookingId) : IRequest<BookingResponse?>;
