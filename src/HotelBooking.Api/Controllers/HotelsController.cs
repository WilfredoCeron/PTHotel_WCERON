using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using HotelBooking.Application.Commands;
using HotelBooking.Application.DTOs;
using HotelBooking.Application.Queries;
using HotelBooking.Domain.Common;

namespace HotelBooking.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class HotelsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly HotelBooking.Infrastructure.Repositories.IdempotencyRepository _idempotencyRepository;
        private readonly HotelBooking.Infrastructure.Data.HotelBookingDbContext _dbContext;
        public HotelsController(IMediator mediator, HotelBooking.Infrastructure.Repositories.IdempotencyRepository idempotencyRepository, HotelBooking.Infrastructure.Data.HotelBookingDbContext dbContext)
        {
            _mediator = mediator;
            _idempotencyRepository = idempotencyRepository;
            _dbContext = dbContext;
        }

        [HttpGet("search-availability")]
        public async Task<IActionResult> SearchAvailability([FromQuery] string? city, [FromQuery] DateTime? checkIn, [FromQuery] DateTime? checkOut, [FromQuery] int? guests)
        {
            // Validación básica
            if (checkIn == null || checkOut == null || checkIn > checkOut || guests == null || guests <= 0)
                return BadRequest("Parámetros inválidos");

            // Obtener hoteles activos
            var query = new GetHotelsQuery(1, 1000); // Traer muchos hoteles
            var result = await _mediator.Send(query);
            var hotels = result.Data.Where(h => h.IsActive);
            if (!string.IsNullOrEmpty(city))
                hotels = hotels.Where(h => h.City.ToLower().Contains(city.ToLower()));

            // Acceso a la base de datos para RoomTypes y RoomInventory
            using var scope = HttpContext.RequestServices.CreateScope();
            var db = (HotelBooking.Infrastructure.Data.HotelBookingDbContext)scope.ServiceProvider.GetService(typeof(HotelBooking.Infrastructure.Data.HotelBookingDbContext));

            var availableHotels = new List<object>();
            foreach (var hotel in hotels)
            {
                // Buscar RoomTypes activos con capacidad suficiente
                var roomTypes = db.RoomTypes.Where(rt => rt.HotelId == hotel.Id && rt.IsActive && rt.Capacity >= guests).ToList();
                bool hotelHasAvailability = false;
                foreach (var roomType in roomTypes)
                {
                    // Para cada RoomType, verificar disponibilidad en todas las fechas del rango
                    bool availableAllDates = true;
                    for (var date = checkIn.Value.Date; date < checkOut.Value.Date; date = date.AddDays(1))
                    {
                        var inventory = db.RoomInventories.FirstOrDefault(ri => ri.RoomTypeId == roomType.Id && ri.Date == date);
                        if (inventory == null || inventory.AvailableRooms < 1)
                        {
                            availableAllDates = false;
                            break;
                        }
                    }
                    if (availableAllDates)
                    {
                        hotelHasAvailability = true;
                        break;
                    }
                }
                if (hotelHasAvailability)
                {
                    availableHotels.Add(new {
                        hotel.Id,
                        hotel.Name,
                        hotel.City,
                        hotel.Country,
                        hotel.Address,
                        hotel.IsActive
                    });
                }
            }
            return Ok(availableHotels);
        }
        [HttpPatch("{id}/activate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ActivateHotel(int id)
        {
            var hotel = await _mediator.Send(new GetHotelByIdQuery(id));
            if (hotel == null)
                return NotFound();

            var command = new ActivateHotelCommand(id);
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.ErrorMessage, code = result.ErrorCode });
            return Ok();
        }

        [HttpPatch("{id}/inactivate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> InactivateHotel(int id)
        {
            var hotel = await _mediator.Send(new GetHotelByIdQuery(id));
            if (hotel == null)
                return NotFound();

            var command = new InactivateHotelCommand(id);
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.ErrorMessage, code = result.ErrorCode });
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateHotel([FromBody] CreateHotelDto dto)
        {
            // Leer Idempotency-Key
            var idempotencyKey = Request.Headers["Idempotency-Key"].FirstOrDefault();
            if (!string.IsNullOrEmpty(idempotencyKey))
            {
                var existingRecord = await _idempotencyRepository.GetByKeyAsync(idempotencyKey);
                if (existingRecord != null && !existingRecord.IsExpired)
                {
                    // Devolver respuesta guardada
                    return StatusCode(existingRecord.ResponseStatusCode, existingRecord.ResponseBody);
                }
            }

            var command = new CreateHotelCommand(
                dto.Name,
                dto.Address,
                dto.City,
                dto.Country,
                dto.Phone,
                dto.Email,
                dto.Latitude,
                dto.Longitude
            );
            var result = await _mediator.Send(command);
            IActionResult response;
            int statusCode;
            string responseBody;
            if (!result.IsSuccess)
            {
                response = BadRequest(new { error = result.ErrorMessage, code = result.ErrorCode });
                statusCode = StatusCodes.Status400BadRequest;
                responseBody = System.Text.Json.JsonSerializer.Serialize(new { error = result.ErrorMessage, code = result.ErrorCode });
            }
            else
            {
                response = CreatedAtAction(nameof(GetHotel), new { id = result.Data }, result.Data);
                statusCode = StatusCodes.Status201Created;
                responseBody = System.Text.Json.JsonSerializer.Serialize(result.Data);
            }

            // Guardar registro de idempotencia
            if (!string.IsNullOrEmpty(idempotencyKey))
            {
                var record = HotelBooking.Domain.Entities.IdempotencyRecord.Create(
                    idempotencyKey,
                    statusCode,
                    responseBody
                );
                await _dbContext.IdempotencyRecords.AddAsync(record);
                await _dbContext.SaveChangesAsync();
            }

            return response;
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateHotel(int id, [FromBody] UpdateHotelDto dto)
        {
            var command = new UpdateHotelCommand(id, dto.Name, dto.Address, dto.City, dto.Country, dto.Phone, dto.Email);
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
            {
                if (result.ErrorCode == "HOTEL_NOT_FOUND")
                    return NotFound();
                return BadRequest(new { error = result.ErrorMessage, code = result.ErrorCode });
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            var command = new DeleteHotelCommand(id);
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
            {
                if (result.ErrorCode == "HOTEL_NOT_FOUND")
                    return NotFound();
                return BadRequest(new { error = result.ErrorMessage, code = result.ErrorCode });
            }
            return Ok();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(HotelDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<HotelDto>> GetHotel(int id)
        {
            var query = new GetHotelByIdQuery(id);
            var hotel = await _mediator.Send(query);
            if (hotel == null)
                return NotFound();
            return Ok(hotel);
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<HotelDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<HotelDto>>> GetHotels(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetHotelsQuery(pageNumber, pageSize);
            var result = await _mediator.Send(query);
            return Ok(result.Data);
        }
    }
}
