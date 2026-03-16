using Booking.Orchestrator.Application.Commands;
using Booking.Orchestrator.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Orchestrator.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly IMediator _mediator;

    public BookingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingCommand command)
    {
        var response = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetBookingStatus), new { id = response.BookingId }, response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookingStatus(Guid id)
    {
        var response = await _mediator.Send(new GetBookingStatusQuery(id));

        if (response is null)
            return NotFound();

        return Ok(response);
    }
}
