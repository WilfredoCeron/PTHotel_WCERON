using HotelBooking.Domain.Common;

namespace HotelBooking.Domain.Entities;

public class RatePlan : Entity
{
    public int RoomTypeId { get; private set; }
    public decimal Price { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public bool IsActive { get; private set; }
    
    // Navigation
    public RoomType? RoomType { get; private set; }
    
    private RatePlan() { }
    
    public static RatePlan Create(int roomTypeId, decimal price, DateTime startDate, DateTime endDate)
    {
        if (price <= 0)
            throw new ArgumentException("El precio debe ser mayor que 0.");
        if (endDate <= startDate)
            throw new ArgumentException("La fecha de finalización debe ser posterior a la fecha de inicio");
        return new RatePlan
        {
            RoomTypeId = roomTypeId,
            Price = price,
            StartDate = startDate.Date,
            EndDate = endDate.Date,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }
    
    public decimal CalculateTotalPrice(int nights)
    {
        return Price * nights;
    }
    
    public bool IsValidForDate(DateTime date)
    {
        var checkDate = date.Date;
        return IsActive && checkDate >= StartDate && checkDate <= EndDate;
    }
}
