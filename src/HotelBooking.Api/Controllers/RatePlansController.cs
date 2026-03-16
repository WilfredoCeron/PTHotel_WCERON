using Microsoft.AspNetCore.Mvc;
using HotelBooking.Infrastructure.Data;
using HotelBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Api.Controllers;

[ApiController]
[Route("api/v1/rateplans")]
public class RatePlansController : ControllerBase
{
    private readonly HotelBookingDbContext _db;
    public RatePlansController(HotelBookingDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var ratePlans = await _db.RatePlans.Include(r => r.RoomType).ToListAsync();
        return Ok(ratePlans);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var ratePlan = await _db.RatePlans.FindAsync(id);
        if (ratePlan == null) return NotFound();
        return Ok(ratePlan);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RatePlan dto)
    {
        _db.RatePlans.Add(dto);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] RatePlan dto)
    {
        var existing = await _db.RatePlans.FindAsync(id);
        if (existing == null) return NotFound();
        existing.GetType().GetProperty("Price")?.SetValue(existing, dto.Price);
        existing.GetType().GetProperty("StartDate")?.SetValue(existing, dto.StartDate);
        existing.GetType().GetProperty("EndDate")?.SetValue(existing, dto.EndDate);
        existing.GetType().GetProperty("IsActive")?.SetValue(existing, dto.IsActive);
        await _db.SaveChangesAsync();
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _db.RatePlans.FindAsync(id);
        if (existing == null) return NotFound();
        _db.RatePlans.Remove(existing);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
