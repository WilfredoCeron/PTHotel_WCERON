using HotelBooking.Application.DTOs;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Controllers
{
    [ApiController]
    [Route("api/v1/room-types")]
    public class RoomTypesController : ControllerBase
    {
        private readonly IRepository<RoomType> _repository;
        private readonly HotelBooking.Infrastructure.Data.HotelBookingDbContext _db;

        public RoomTypesController(IRepository<RoomType> repository, HotelBooking.Infrastructure.Data.HotelBookingDbContext db)
        {
            _repository = repository;
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? hotelId = null)
        {
            var roomTypes = await _repository.GetAllAsync();
            if (hotelId.HasValue)
            {
                var filtered = roomTypes.Where(rt => rt.HotelId == hotelId.Value);
                return Ok(filtered);
            }
            return Ok(roomTypes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var roomType = await _repository.GetByIdAsync(id);
            if (roomType == null) return NotFound();
            return Ok(roomType);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RoomType dto)
        {
            // Validar nombre único por hotel
            var exists = _db.RoomTypes.Any(rt => rt.HotelId == dto.HotelId && rt.Name.ToLower() == dto.Name.ToLower());
            if (exists)
            {
                return Conflict(new { message = "Ya existe un tipo de habitación con ese nombre en este hotel." });
            }

            // Idempotencia: buscar o crear registro de idempotencia
            var idempotencyKey = Request.Headers["Idempotency-Key"].FirstOrDefault();
            if (string.IsNullOrEmpty(idempotencyKey))
            {
                await _repository.AddAsync(dto);
                await _db.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
            }

            var existingIdem = _db.IdempotencyRecords.FirstOrDefault(r => r.Key == idempotencyKey);
            if (existingIdem != null)
            {
                // Ya existe, devolver el último RoomType creado con esa key
                var lastRoomType = _db.RoomTypes.OrderByDescending(r => r.Id).FirstOrDefault();
                return Conflict(new { message = "Ya existe un registro con esta clave de idempotencia", roomType = lastRoomType });
            }
            // Registrar la clave de idempotencia usando el método Create
            var idemRecord = HotelBooking.Domain.Entities.IdempotencyRecord.Create(
                idempotencyKey,
                201,
                "RoomType creado"
            );
            _db.IdempotencyRecords.Add(idemRecord);

            await _repository.AddAsync(dto);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RoomType dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            // Validar nombre único por hotel (excluyendo el propio id)
            var exists = _db.RoomTypes.Any(rt => rt.HotelId == (dto.HotelId != 0 ? dto.HotelId : existing.HotelId)
                && rt.Name.ToLower() == dto.Name.ToLower()
                && rt.Id != id);
            if (exists)
            {
                return Conflict(new { message = "Ya existe un tipo de habitación con ese nombre en este hotel." });
            }

            // Actualiza propiedades
            existing.Name = dto.Name;
            existing.Description = dto.Description;
            // Si el hotelId viene en el body, lo actualiza, si no, mantiene el actual
            if (dto.HotelId != 0) existing.HotelId = dto.HotelId;
            existing.Capacity = dto.Capacity;
            existing.TotalRooms = dto.TotalRooms;
            _repository.Update(existing);
            await _db.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _repository.Delete(existing);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
