using HotelBooking.Domain.Common;

namespace HotelBooking.Domain.Entities;

public enum BookingStatus
{
    Pending = 1,
    Confirmed = 2,
    Cancelled = 3,
    Completed = 4
}

public class Booking : Entity
{
    public int HotelId { get; set; }
    public int RoomTypeId { get; set; }
    public int GuestId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NumberOfRooms { get; set; }
    public int NumberOfGuests { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; }
    public string? SpecialRequests { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    public byte[] RowVersion { get; set; } = null!; // Concurrency token
    
    // Navigation
    public Hotel? Hotel { get; set; }
    public RoomType? RoomType { get; set; }
    public Guest? Guest { get; set; }
    public ICollection<Payment> Payments { get; private set; } = new List<Payment>();
    
    public Booking() { }
    
    public static Booking Create(int hotelId, int roomTypeId, int guestId, 
        DateTime checkInDate, DateTime checkOutDate, int numberOfRooms, int numberOfGuests, 
        decimal totalPrice, string? specialRequests = null)
    {
        if (checkOutDate <= checkInDate)
            throw new ArgumentException("el Check-out date debe ser posterior a la fecha de check-in");
        
        var nights = (int)(checkOutDate.Date - checkInDate.Date).TotalDays;
        if (nights <= 0)
            throw new ArgumentException("La reserva debe ser al menos 1 noche");
        
        if (numberOfRooms <= 0)
            throw new ArgumentException("El número de habitaciones debe ser mayor que 0");
        
        if (numberOfGuests <= 0)
            throw new ArgumentException("El número de huéspedes debe ser mayor que 0");
        
        var booking = new Booking
        {
            HotelId = hotelId,
            RoomTypeId = roomTypeId,
            GuestId = guestId,
            CheckInDate = checkInDate.Date,
            CheckOutDate = checkOutDate.Date,
            NumberOfRooms = numberOfRooms,
            NumberOfGuests = numberOfGuests,
            TotalPrice = totalPrice,
            Status = BookingStatus.Pending,
            SpecialRequests = specialRequests,
            CreatedAt = DateTime.UtcNow
        };
        booking.AddDomainEvent(new BookingCreatedEvent(booking.Id, hotelId, guestId, 
            checkInDate, checkOutDate, numberOfRooms));
        return booking;
    }
    
    public void Confirm()
    {
        if (Status != BookingStatus.Pending)
            throw new InvalidOperationException("Sólo se pueden confirmar reservas pendientes");
        
        Status = BookingStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        
        AddDomainEvent(new BookingConfirmedEvent(Id, HotelId, GuestId));
    }
    
    public void Cancel(string? reason = null)
    {
        if (Status == BookingStatus.Cancelled || Status == BookingStatus.Completed)
            throw new InvalidOperationException("No se puede cancelar una reserva completada o ya cancelada");
        
        Status = BookingStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        CancellationReason = reason;
        UpdatedAt = DateTime.UtcNow;
        
        AddDomainEvent(new BookingCancelledEvent(Id, HotelId, GuestId, reason));
    }
}

// Domain Events
public class BookingCreatedEvent : IDomainEvent
{
    public int AggregateId { get; }
    public DateTime OccurredAt { get; }
    public int HotelId { get; }
    public int GuestId { get; }
    public DateTime CheckInDate { get; }
    public DateTime CheckOutDate { get; }
    public int NumberOfRooms { get; }
    
        public BookingCreatedEvent(int bookingId, int hotelId, int guestId, 
        DateTime checkInDate, DateTime checkOutDate, int numberOfRooms)
    {
        AggregateId = bookingId;
        HotelId = hotelId;
        GuestId = guestId;
        CheckInDate = checkInDate;
        CheckOutDate = checkOutDate;
        NumberOfRooms = numberOfRooms;
        OccurredAt = DateTime.UtcNow;
    }
}

public class BookingConfirmedEvent : IDomainEvent
{
    public int AggregateId { get; }
    public DateTime OccurredAt { get; }
    public int HotelId { get; }
    public int GuestId { get; }
    
        public BookingConfirmedEvent(int bookingId, int hotelId, int guestId)
    {
        AggregateId = bookingId;
        HotelId = hotelId;
        GuestId = guestId;
        OccurredAt = DateTime.UtcNow;
    }
}

public class BookingCancelledEvent : IDomainEvent
{
    public int AggregateId { get; }
    public DateTime OccurredAt { get; }
    public int HotelId { get; }
    public int GuestId { get; }
    public string? Reason { get; }
    
        public BookingCancelledEvent(int bookingId, int hotelId, int guestId, string? reason = null)
    {
        AggregateId = bookingId;
        HotelId = hotelId;
        GuestId = guestId;
        Reason = reason;
        OccurredAt = DateTime.UtcNow;
    }
}
