using HotelBooking.Domain.Common;

namespace HotelBooking.Domain.Entities;

public class Guest : Entity
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? IdentificationNumber { get; set; }
    public bool IsActive { get; set; }
    // Navigation
    public ICollection<Booking> Bookings { get; private set; } = new List<Booking>();
    public Guest() { } 
    
    public static Guest Create(string firstName, string lastName, string email, 
        string? phoneNumber = null, string? identificationNumber = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("Nombre es requerido");
        
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Apellido es requerido");
        
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email es requerido");
        
        return new Guest
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PhoneNumber = phoneNumber,
            IdentificationNumber = identificationNumber,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }
    
    public void Update(string firstName, string lastName, string? phoneNumber = null)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        UpdatedAt = DateTime.UtcNow;
    }
}
