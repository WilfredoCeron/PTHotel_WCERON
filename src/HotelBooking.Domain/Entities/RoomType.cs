using HotelBooking.Domain.Common;

namespace HotelBooking.Domain.Entities;

public class RoomType : Entity
{
    public int HotelId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int Capacity { get; set; }
    public int TotalRooms { get; set; }
    public bool IsActive { get; set; }
    

    public Hotel? Hotel { get; set; }
    public ICollection<RoomInventory> RoomInventories { get; set; } = new List<RoomInventory>();
    public ICollection<RatePlan> RatePlans { get; set; } = new List<RatePlan>();
    
    public RoomType() { }
    public static RoomType Create(int hotelId, string name, string description, int capacity, int totalRooms)
    {
        if (capacity <= 0)
            throw new ArgumentException("La capacidad debe ser mayor que 0.");
        if (totalRooms <= 0)
            throw new ArgumentException("El total de habitaciones debe ser mayor que 0.");
        return new RoomType
        {
            HotelId = hotelId,
            Name = name,
            Description = description,
            Capacity = capacity,
            TotalRooms = totalRooms,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }
    
    public void Update(string name, string description, int capacity)
    {
        Name = name;
        Description = description;
        Capacity = capacity;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
