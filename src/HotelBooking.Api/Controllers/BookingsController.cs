using Microsoft.AspNetCore.Mvc;
using MediatR;
using HotelBooking.Application.Commands;
using HotelBooking.Application.DTOs;

namespace HotelBooking.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public BookingsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<int>> CreateBooking(
        [FromBody] CreateBookingDto dto,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey = null)
    {
        var command = new CreateBookingCommand(
            dto.HotelId,
            dto.RoomTypeId,
            dto.GuestId,
            dto.CheckInDate,
            dto.CheckOutDate,
            dto.NumberOfRooms,
            dto.NumberOfGuests,
            dto.SpecialRequests,
            idempotencyKey
        );
        
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            if (result.ErrorCode == "OVERBOOKING")
                return Conflict(new { error = result.ErrorMessage, code = result.ErrorCode });
            return BadRequest(new { error = result.ErrorMessage, code = result.ErrorCode });
        }
        
        return CreatedAtAction(nameof(GetBooking), new { id = result.Data }, result.Data);
    }
    
    [HttpPut("{id}/confirm")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmBooking(int id)
    {
        var command = new ConfirmBookingCommand(id);
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            if (result.ErrorCode == "BOOKING_NOT_FOUND")
                return NotFound();
            return BadRequest(new { error = result.ErrorMessage, code = result.ErrorCode });
        }
        
        return Ok();
    }
    
    [HttpDelete("{id}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelBooking(int id, [FromQuery] string? reason = null)
    {
        var command = new CancelBookingCommand(id, reason);
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            if (result.ErrorCode == "BOOKING_NOT_FOUND")
                return NotFound();
            return BadRequest(new { error = result.ErrorMessage, code = result.ErrorCode });
        }
        
        return Ok();
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookingDto>> GetBooking(int id)
    {
        // Placeholder - will use Dapper in Infrastructure
        return NotFound();
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<BookingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<BookingDto>>> GetBookings(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = "CreatedAt",
        [FromQuery] string? sortDirection = "DESC")
    {
        var query = new HotelBooking.Application.Queries.GetBookingsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortBy = sortBy,
            SortDirection = sortDirection
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
