using Microsoft.AspNetCore.Mvc;
using MediatR;
using HotelBooking.Application.Commands;
using HotelBooking.Application.DTOs;

namespace HotelBooking.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class GuestsController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public GuestsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<int>> CreateGuest([FromBody] CreateGuestDto dto)
    {
        var command = new CreateGuestCommand(
            dto.FirstName,
            dto.LastName,
            dto.Email,
            dto.PhoneNumber,
            dto.IdentificationNumber
        );
        
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
            return BadRequest(new { error = result.ErrorMessage, code = result.ErrorCode });
        
        return CreatedAtAction(nameof(GetGuest), new { id = result.Data }, result.Data);
    }
    
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateGuest(int id, string firstName, string lastName, string? phoneNumber = null)
    {
        var command = new UpdateGuestCommand(id, firstName, lastName, phoneNumber);
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            if (result.ErrorCode == "GUEST_NOT_FOUND")
                return NotFound();
            return BadRequest(new { error = result.ErrorMessage, code = result.ErrorCode });
        }
        
        return Ok();
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GuestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GuestDto>> GetGuest(int id)
    {
        // Placeholder - will use Dapper in Infrastructure
        return NotFound();
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<GuestDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<GuestDto>>> GetGuests(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        // Placeholder - will use Dapper in Infrastructure
        return Ok(new PaginatedResult<GuestDto>
        {
            Data = new List<GuestDto>(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = 0
        });
    }
}
