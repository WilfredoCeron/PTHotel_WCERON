using HotelBooking.Domain.Common;

namespace HotelBooking.Domain.Entities;

public class Hotel : Entity
{
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Country { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public bool IsActive { get; set; }
    
    public ICollection<RoomType> RoomTypes { get; set; } = new List<RoomType>();
    public Hotel() { }
    public static Hotel Create(string name, string address, string city, string country, 
        string? phone = null, string? email = null, decimal? latitude = null, decimal? longitude = null)
    {
        return new Hotel
        {
            Name = name,
            Address = address,
            City = city,
            Country = country,
            Phone = phone,
            Email = email,
            Latitude = latitude,
            Longitude = longitude,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }
    
    public void UpdateInfo(string name, string address, string city, string country, 
        string? phone = null, string? email = null)
    {
        Name = name;
        Address = address;
        City = city;
        Country = country;
        Phone = phone;
        Email = email;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
