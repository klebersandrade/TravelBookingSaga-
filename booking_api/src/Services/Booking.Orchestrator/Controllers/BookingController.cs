using Booking.Orchestrator.Application.Commands;
using Booking.Orchestrator.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Orchestrator.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController: ControllerBase
{
    private readonly BookingSagaOrchestrator _orchestrator;

    public BookingController(BookingSagaOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }

    [HttpPost]
    public IActionResult CreateBooking([FromBody] CreateBookingCommand command)
    {
        var response = _orchestrator.StartSaga(command);
        return CreatedAtAction(nameof(GetBookingStatus), new { id = response.BookingId }, response);
    }

    [HttpGet("{id}")]
    public IActionResult GetBookingStatus(Guid id)
    {
        // TODO: Get saga status from database
        throw new NotImplementedException();
    }
}
