using Microsoft.AspNetCore.Mvc;
using HotelBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Api.Controllers;

[ApiController]
[Route("api/v1/rateplans/pricing")]
public class RatePlansPricingController : ControllerBase
{
    private readonly HotelBookingDbContext _db;
    public RatePlansPricingController(HotelBookingDbContext db)
    {
        _db = db;
    }

    // GET: api/v1/rateplans/pricing?roomTypeId=1&checkIn=2026-03-15&checkOut=2026-03-20
    [HttpGet]
    public async Task<IActionResult> GetPrice([FromQuery] int roomTypeId, [FromQuery] DateTime checkIn, [FromQuery] DateTime checkOut)
    {
        if (checkIn >= checkOut)
            return BadRequest("Fechas inválidas");
        var ratePlans = await _db.RatePlans
            .Where(rp => rp.RoomTypeId == roomTypeId && rp.IsActive && rp.StartDate <= checkIn && rp.EndDate >= checkOut)
            .ToListAsync();
        if (!ratePlans.Any())
            return NotFound("No hay tarifa disponible para ese rango de fechas");
        // Tomar la tarifa más barata disponible
        var bestRate = ratePlans.OrderBy(rp => rp.Price).First();
        var nights = (checkOut - checkIn).Days;
        var total = bestRate.Price * nights;
        return Ok(new { pricePerNight = bestRate.Price, nights, total, ratePlanId = bestRate.Id });
    }
}
