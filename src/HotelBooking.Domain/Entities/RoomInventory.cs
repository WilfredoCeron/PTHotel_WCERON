using HotelBooking.Domain.Common;

namespace HotelBooking.Domain.Entities;

public class RoomInventory : Entity
{
    public int RoomTypeId { get; set; }
    public DateTime Date { get; set; }
    public int AvailableRooms { get; set; }
    public int ReservedRooms { get; set; }
    public byte[] RowVersion { get; set; } = null!;
    // Navigation
    public RoomType? RoomType { get; set; }
    public RoomInventory() { }
    
    public static RoomInventory Create(int roomTypeId, DateTime date, int availableRooms)
    {
        if (availableRooms < 0)
            throw new ArgumentException("Las habitaciones disponibles no pueden ser negativas.");
        
        return new RoomInventory
        {
            RoomTypeId = roomTypeId,
            Date = date.Date,
            AvailableRooms = availableRooms,
            ReservedRooms = 0,
            CreatedAt = DateTime.UtcNow
        };
    }
    
    public bool TryReserveRooms(int roomCount)
    {
        if (roomCount <= 0)
            return false;
        
        if (AvailableRooms < roomCount)
            return false;
        
        AvailableRooms -= roomCount;
        ReservedRooms += roomCount;
        UpdatedAt = DateTime.UtcNow;
        return true;
    }
    
    public void ReleaseRooms(int roomCount)
    {
        if (roomCount <= 0)
            return;
        
        if (ReservedRooms >= roomCount)
        {
            ReservedRooms -= roomCount;
            AvailableRooms += roomCount;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
