namespace HotelBooking.Application.DTOs;

public record HotelDto
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string Address { get; init; } = null!;
    public string City { get; init; } = null!;
    public string Country { get; init; } = null!;
    public string? Phone { get; init; }
    public string? Email { get; init; }
    public decimal? Latitude { get; init; }
    public decimal? Longitude { get; init; }
    public bool IsActive { get; init; }
}

public record CreateHotelDto
{
    public string Name { get; init; } = null!;
    public string Address { get; init; } = null!;
    public string City { get; init; } = null!;
    public string Country { get; init; } = null!;
    public string? Phone { get; init; }
    public string? Email { get; init; }
    public decimal? Latitude { get; init; }
    public decimal? Longitude { get; init; }
}

public record UpdateHotelDto
{
    public string Name { get; init; } = null!;
    public string Address { get; init; } = null!;
    public string City { get; init; } = null!;
    public string Country { get; init; } = null!;
    public string? Phone { get; init; }
    public string? Email { get; init; }
}

public record RoomTypeDto
{
    public int Id { get; init; }
    public int HotelId { get; init; }
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
    public int Capacity { get; init; }
    public int TotalRooms { get; init; }
    public bool IsActive { get; init; }
}

public record CreateRoomTypeDto
{
    public int HotelId { get; init; }
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
    public int Capacity { get; init; }
    public int TotalRooms { get; init; }
}

public record BookingDto
{
    public int Id { get; init; }
    public int HotelId { get; init; }
    public int RoomTypeId { get; init; }
    public int GuestId { get; init; }
    public DateTime CheckInDate { get; init; }
    public DateTime CheckOutDate { get; init; }
    public int NumberOfRooms { get; init; }
    public int NumberOfGuests { get; init; }
    public decimal TotalPrice { get; init; }
    public string Status { get; init; } = null!;
    public string? SpecialRequests { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ConfirmedAt { get; init; }
    public DateTime? CancelledAt { get; init; }
}

public record CreateBookingDto
{
    public int HotelId { get; init; }
    public int RoomTypeId { get; init; }
    public int GuestId { get; init; }
    public DateTime CheckInDate { get; init; }
    public DateTime CheckOutDate { get; init; }
    public int NumberOfRooms { get; init; }
    public int NumberOfGuests { get; init; }
    public string? SpecialRequests { get; init; }
}

public record GuestDto
{
    public int Id { get; init; }
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? PhoneNumber { get; init; }
    public bool IsActive { get; init; }
}

public record CreateGuestDto
{
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? PhoneNumber { get; init; }
    public string? IdentificationNumber { get; init; }
}

public record AvailabilityDto
{
    public int HotelId { get; init; }
    public int RoomTypeId { get; init; }
    public int AvailableRooms { get; init; }
    public DateTime Date { get; init; }
}

public record PaginatedResult<T>
{
    public List<T> Data { get; init; } = null!;
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
}

public record RatePlanDto
{
    public int Id { get; init; }
    public int RoomTypeId { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public bool IsActive { get; init; }
}

public record PaymentDto
{
    public int Id { get; init; }
    public int BookingId { get; init; }
    public decimal Amount { get; init; }
    public string Status { get; init; } = null!;
    public string Method { get; init; } = null!;
}
